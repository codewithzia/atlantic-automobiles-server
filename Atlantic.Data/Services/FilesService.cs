using Atlantic.Data.Models.Settings;
using Atlantic.Data.MongoFs;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Atlantic.Data.Services
{
    public class FilesService
    {
        private IMongoCollection<MongoFileObject> _photos;
        private GridFSBucket _gridFs;

        public FilesService(IOptions<DbSettings> options)
        {
            //var client = new MongoClient(settings.ConnectionString);
            //var client = new MongoClient("mongodb+srv://DevTeamMontrims:MondolMont123@cluster0.4dzlr.mongodb.net/ebsCommon?retryWrites=true&w=majority");
            var client = new MongoClient(options.Value.ConnectionStrings);

            //var database = client.GetDatabase(settings.DatabaseName);
            var database = client.GetDatabase(options.Value.Database.Name);

            _gridFs = new GridFSBucket(database);
            //_photos = database.GetCollection<FileObject>(settings.PhotosCollectionName);
            _photos = database.GetCollection<MongoFileObject>(options.Value.Database.Name);
        }


        public async Task<ObjectId> UploadFile(Stream stream, string fileName)
        {

            var id = await _gridFs.UploadFromStreamAsync(fileName, stream);

            string FilePath = Directory.GetCurrentDirectory() + "\\Uploads";

            await SaveFile(id, FilePath, fileName);

            return id;

        }




        public async Task<GridFSDownloadStream> DownloadFile(ObjectId id)
        {

            var dataStream = await _gridFs.OpenDownloadStreamAsync(id);

            return dataStream;

        }

        public async Task SaveFile(ObjectId id, string path, string fileName)
        {

            var photoStream = await DownloadFile(id);

            if (photoStream != null)
            {
                using (Stream output = new FileStream(path + "\\" + fileName, FileMode.Create))
                {
                    byte[] buffer = new byte[32 * 1024];
                    int read;

                    while ((read = photoStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, read);
                    }
                }
            }
        }

        public async Task SaveFile(string id, string path)
        {

            MongoFileObject photo = Get(id);

            if (photo != null)
            {
                var photoStream = await DownloadFile(photo.PhotoObjectId);

                if (photoStream != null)
                {
                    using (Stream output = new FileStream(path + photo.FileName, FileMode.Create))
                    {
                        byte[] buffer = new byte[32 * 1024];
                        int read;

                        while ((read = photoStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, read);
                        }
                    }
                }
            }

        }



        public List<MongoFileObject> Get() =>
            _photos.Find(photo => true).ToList();

        public MongoFileObject Get(string id) =>
            _photos.Find<MongoFileObject>(photo => photo.Id == id).FirstOrDefault();

        public MongoFileObject Create(MongoFileObject photo)
        {
            _photos.InsertOne(photo);
            return photo;
        }

        public void Update(string id, MongoFileObject photoIn) =>
            _photos.ReplaceOne(photo => photo.Id == id, photoIn);

        public void Remove(MongoFileObject photoIn) =>
            _photos.DeleteOne(photo => photo.Id == photoIn.Id);

        public void Remove(string id) =>
            _photos.DeleteOne(photo => photo.Id == id);
    }
}
