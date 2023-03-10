using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Services.Abstract;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections.Generic;

namespace FreeCourse.Services.Catalog.Services.Concreate
{
    internal class CourseService : ICourseService
    {
        private readonly IMongoCollection<Course> _courseCollection;
        private readonly IMongoCollection<Category> _categoryCollection;

        private readonly IMapper _mapper;

        public CourseService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);

            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
        }

        public async Task<Response<List<CourseDto>>> GetAllAsync()
        {
            var courses = await _courseCollection.Find(course => true).ToListAsync();

            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find(x => x.Id == course.CategoryId).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }

            var coursesDto = _mapper.Map<List<CourseDto>>(courses);

            return Response<List<CourseDto>>.Success(coursesDto, 200);
        }

        public async Task<Response<CourseDto>> GetByIdAsync(string id)
        {
            var course = await _courseCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (course == null)
            {
                return Response<CourseDto>.Fail("Course Not Found", 404);
            }

            course.Category = await _categoryCollection.Find(x => x.Id == course.CategoryId).FirstAsync();

            var courseDto = _mapper.Map<CourseDto>(course);
            return Response<CourseDto>.Success(courseDto, 200);
        }

        public async Task<Response<List<CourseDto>>> GetByUserIdAsync(string userId)
        {
            var courses = await _courseCollection.Find(x => x.UserId == userId).ToListAsync();

            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find(x => x.Id == course.CategoryId).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }

            var coursesDto = _mapper.Map<List<CourseDto>>(courses);

            return Response<List<CourseDto>>.Success(coursesDto, 200);
        }

        public async Task<Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto)
        {
            var newCourse = _mapper.Map<Course>(courseCreateDto);
            newCourse.CreatedTime = DateTime.Now;

            await _courseCollection.InsertOneAsync(newCourse);

            var courseDto = _mapper.Map<CourseDto>(newCourse);

            return Response<CourseDto>.Success(courseDto, 200);
        }

        public async Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
        {
            var courseUpdate = _mapper.Map<Course>(courseUpdateDto);

            var result = await _courseCollection.FindOneAndReplaceAsync(x => x.Id == courseUpdate.Id, courseUpdate);

            if (result == null)
            {
                return Response<NoContent>.Fail("Course Not Found", 404);
            }
            else
            {
                return Response<NoContent>.Success(200);
            }
        }

        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            var result = await _courseCollection.FindOneAndDeleteAsync(x => x.Id == id);

            if (result == null)
            {
                return Response<NoContent>.Fail("Course Not Found", 404);
            }
            else
            {
                return Response<NoContent>.Success(200);
            }
        }
    }
}
