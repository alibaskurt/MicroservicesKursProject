using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Services.Abstract;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services.Concreate
{
    //Burada Db den gelen category bilgilerini mapping yapacağımız için adına Repositor değilde Service dedim.
    //Daha profesyonel olması adına once ICategoryRepository vs oluşturulup service üzerinden bu interface çağrılabilirdi.
    internal class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;

        public CategoryService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
        }

        public async Task<Response<List<CategoryDto>>> GetAllAsync()
        {
            //Veri tabanından kategori listesini çek.
            var categories = await _categoryCollection.Find(category => true).ToListAsync();

            //Response Success methoduna çekilen kategori listesi Dto nesnsi olarak map lenip verilecek.
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);

            return Response<List<CategoryDto>>.Success(categoriesDto, 200);
        }

        public async Task<Response<CategoryDto>> CreateAsync(Category category)
        {
            await _categoryCollection.InsertOneAsync(category);

            var categoryDto = _mapper.Map<CategoryDto>(category);

            return Response<CategoryDto>.Success(categoryDto, 200);
        }

        public async Task<Response<CategoryDto>> GetByIdAsync(string id)
        {
            var category = await _categoryCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (category == null)
                return Response<CategoryDto>.Fail("Category Not Found", 404);

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Response<CategoryDto>.Success(categoryDto, 200);
        }


    }
}
