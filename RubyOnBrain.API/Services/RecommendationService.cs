using Microsoft.EntityFrameworkCore;
using RubyOnBrain.API.Models;
using RubyOnBrain.Data;
using RubyOnBrain.Domain;
using System.Text.Json;

namespace RubyOnBrain.API.Services
{
    public class RecommendationService
    {
        private readonly string uploadPath;
        private List<UserCoursePredictsDTO>? lastPredicts;

        public RecommendationService()
        {
            lastPredicts = new List<UserCoursePredictsDTO>();

            uploadPath = $"wwwroot/uploads/data/";

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);
        }

        public bool SavePredicts(List<UserCoursePredictsDTO> predicts) 
        {
            try
            {
                lastPredicts = predicts;
                SaveToFile(predicts);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<string>? GetFileNames()
        {
            var fileNames = Directory.GetFiles(uploadPath, "*.json").ToList();
            return fileNames;
        }

        public async void SaveToFile(List<UserCoursePredictsDTO> predicts)
        {
            string fullPath = uploadPath + $"recsyspredicts_{DateTime.Now.Date.ToShortDateString()}.json";
            using (FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate))
            { 
                await JsonSerializer.SerializeAsync(fs, predicts);
            }
        }

        public bool LoadRatingsFromFile(string fileName)
        {
            if (File.Exists(uploadPath + $"/{fileName}"))
            {
                try
                {
                    ReadFromFile(uploadPath + $"/{fileName}");
                    return true;
                }
                catch { }
            }
            
            return false;
        }

        public async void ReadFromFile(string filePath)
        {
            List<UserCoursePredictsDTO>? predicts;
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                predicts = await JsonSerializer.DeserializeAsync<List<UserCoursePredictsDTO>>(fs);
            }

            if (predicts != null)
                lastPredicts = predicts;
        
        }

        public UserCoursePredictsDTO? GetPredictForUser(int id)
        {
            UserCoursePredictsDTO? ucpDTO = null;
            if (lastPredicts != null)
            {
                var ratings = lastPredicts.Where(u => u.UserId == id).Select(x => x.Rating);
                if (ratings.Count() > 0)
                {
                    var maxRating = ratings.Max();

                    ucpDTO = lastPredicts.FirstOrDefault(uc => uc.UserId == id && uc.Rating == maxRating);
                }
                
            }
            return ucpDTO;
        }
    }
}
