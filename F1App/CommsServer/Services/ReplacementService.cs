using DataAccess.Repositories;
using Domain;
using Domain.SearchCriteria;
using Exceptions;
using Protocol;

namespace CommsServer.Services
{
    public class ReplacementService
    {
        private LogService _logService;
        private ReplacementRepository _replacementRepository;

        public ReplacementService(LogService logService)
        {
            _replacementRepository = ReplacementRepository.GetInstance();
            _logService = logService;
        }

        public Frame PostReplacement(Frame requestFrame)
        {
            string[] dataFrame = requestFrame.GetDataParams();
            string replacementName = dataFrame[0];
            string replacementProvider = dataFrame[1];
            string replacementBrand = dataFrame[2];

            Replacement createdReplacement = new Replacement
            {
                Name = replacementName,
                Brand = replacementBrand,
                Provider = replacementProvider
            };

            _replacementRepository.Store(createdReplacement);
            _logService.EmitEntityLog(createdReplacement, requestFrame.Command);

            return new Frame(requestFrame.Command) { Data = "Replacement " + replacementName + " created" };
        }

        public Frame PostReplacementCategory(Frame requestFrame)
        {
            string[] dataFrame = requestFrame.GetDataParams();
            int replacementId = int.Parse(dataFrame[0]);
            string categoryName = dataFrame[1];

            Category createdCategory = new Category { Name = categoryName };

            _replacementRepository.StoreNewCategory(replacementId, createdCategory);
            _logService.EmitEntityLog(createdCategory, requestFrame.Command);

            return new Frame(requestFrame.Command) { Data = "Category " + categoryName + " created" };
        }

        public Frame PostReplacementPhoto(Frame requestFrame)
        {
            string[] dataFrame = requestFrame.GetDataParams();
            int replacementId = int.Parse(dataFrame[0]);

            string originalPhotoPath = dataFrame[1];
            string fileName = originalPhotoPath.Split('\\').Last();

            string projectExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string projectPath = Path.GetDirectoryName(projectExeFilePath);

            string photoPath = projectPath + "\\" + fileName;

            _replacementRepository.StorePhoto(replacementId, photoPath);
            _logService.EmitEntityLog(photoPath, requestFrame.Command);

            return new Frame(requestFrame.Command) { Data = "Photo " + fileName + " uploaded" };
        }

        public Frame ShowReplacement(Frame requestFrame)
        {
            string dataFrame = requestFrame.Data;
            int replacementId = int.Parse(dataFrame);

            var retrievedReplacement = _replacementRepository.Get(replacementId);
            string data = retrievedReplacement.ToString();

            _logService.EmitEntityLog(retrievedReplacement, requestFrame.Command);

            return new Frame(requestFrame.Command) { Data = data };
        }

        public Frame IndexReplacements(Frame requestFrame)
        {
            var retrievedReplacements = _replacementRepository.GetAll();
            _logService.EmitEntityLog(retrievedReplacements, requestFrame.Command);

            var showedReplacements = retrievedReplacements.Select(r => r.ToString()).ToArray();
            string data = string.Join("\n", showedReplacements);

            return new Frame(requestFrame.Command) { Data = data };
        }

        public Frame IndexReplacementsByKeyWords(Frame requestFrame)
        {
            var searchCriteria = new ReplacementSearchByKeyWord();
            var retrievedReplacements = new List<Replacement>();

            string dataFrame = requestFrame.Data;
            string[] separators = new string[] { "," };
            var keywords = dataFrame.Split(separators, StringSplitOptions.None);

            foreach (string keyword in keywords)
            {
                searchCriteria.KeyWord = keyword;
                List<Replacement> criteriaReplacemets = _replacementRepository.GetBy(searchCriteria);
                retrievedReplacements.AddRange(criteriaReplacemets);
            }

            _logService.EmitEntityLog(retrievedReplacements, requestFrame.Command);

            var showedReplacements = retrievedReplacements.Select(r => r.ToString()).ToArray();
            string data = string.Join("\n", showedReplacements);

            return new Frame(requestFrame.Command) { Data = data };
        }

        public Frame ShowReplacementCategories(Frame requestFrame)
        {
            string dataFrame = requestFrame.Data;
            int replacementId = int.Parse(dataFrame);

            var retrievedReplacement = _replacementRepository.Get(replacementId);
            var replacementCategories = retrievedReplacement.Categories;

            _logService.EmitEntityLog(replacementCategories, requestFrame.Command);

            var showedCategories = replacementCategories.Select(m => m.ToString()).ToArray();
            string data = string.Join("\n", showedCategories);

            return new Frame(requestFrame.Command) { Data = data };
        }

        public Frame ShowReplacementPhoto(Frame requestFrame)
        {
            string dataFrame = requestFrame.Data;
            int replacementId = int.Parse(dataFrame);

            var retrievedReplacement = _replacementRepository.Get(replacementId);
            var photoPath = retrievedReplacement.PhotoPath;

            if (photoPath == null)
                throw new ResourceNotFoundException("That replacement has no associated photo");

            _logService.EmitEntityLog(photoPath, requestFrame.Command);

            Frame frameResponse = new Frame(requestFrame.Command);
            string fileName = photoPath.Split('\\').Last();
            string message = "Received replacement photo " + fileName;
            frameResponse.Data = message + Constants.DataSeparator + photoPath;

            return frameResponse;
        }

        public Frame UpdateReplacement(Frame requestFrame)
        {
            string[] dataFrame = requestFrame.GetDataParams();
            int replacementId = int.Parse(dataFrame[0]);
            string replacementName = dataFrame[1];
            string replacementProvider = dataFrame[2];
            string replacementBrand = dataFrame[3];

            Replacement createdReplacement = new Replacement
            {
                Name = replacementName,
                Brand = replacementBrand,
                Provider = replacementProvider
            };

            var updatedReplacement = _replacementRepository.Update(replacementId, createdReplacement);
            _logService.EmitEntityLog(updatedReplacement, requestFrame.Command);

            return new Frame(requestFrame.Command) { Data = "Replacement " + replacementName + " updated" };
        }

        public Frame RemoveReplacement(Frame requestFrame)
        {
            string dataFrame = requestFrame.Data;
            int replacementId = int.Parse(dataFrame);

            _replacementRepository.Delete(replacementId);

            string message = $"Replacement {replacementId} removed";

            _logService.EmitEntityLog(message, requestFrame.Command);

            return new Frame(requestFrame.Command) { Data = message };
        }

        public Frame RemoveReplacementPhoto(Frame requestFrame)
        {
            string dataFrame = requestFrame.Data;
            int replacementId = int.Parse(dataFrame);

            var retrievedReplacement = _replacementRepository.Get(replacementId);
            var photoPath = retrievedReplacement.PhotoPath;

            if (photoPath == null)
                return new Frame(requestFrame.Command) { Data = "Photo already removed" };

            _replacementRepository.RemovePhoto(replacementId);

            string fileName = photoPath.Split('\\').Last();
            string message = $"Photo {fileName} removed";

            _logService.EmitEntityLog(message, requestFrame.Command);

            return new Frame(requestFrame.Command) { Data = message };
        }
    }
}