using RubyOnBrain.API.Models;
using RubyOnBrain.Data;
using RubyOnBrain.Domain;

namespace RubyOnBrain.API.Services
{
    public class EntryService
    {
        private readonly DataContext db;

        public EntryService(DataContext db)
        { 
            this.db = db;
        }

        public List<EntryDTO> GetEntries()
        {
            return db.Entries.Select(e => new EntryDTO() { EntryTypeId = e.EntryTypeId, Id = e.Id, ImgName = e.ImgName, Text = e.Text, Title = e.Title, TopicId = e.TopicId, VideoName = e.VideoName }).ToList();
        }

        public EntryDTO? GetEntry(int id)
        {
            var e = db.Entries.FirstOrDefault(e => e.Id == id);
            if (e != null)
                return new EntryDTO() { EntryTypeId = e.EntryTypeId, Id = e.Id, ImgName = e.ImgName, Text = e.Text, Title = e.Title, TopicId = e.TopicId, VideoName = e.VideoName };
            else
                return null;
        }

        public bool UpdateEntry(EntryDTO entry)
        {
            var findedEntry = db?.Entries.FirstOrDefault(e => e.Id == entry.Id);
            var topic = db?.Topics.FirstOrDefault(t => t.Id == entry.TopicId);
            var entryType = db?.EntryTypes.FirstOrDefault(et => et.Id == entry.EntryTypeId);

            if (findedEntry != null && topic != null && entryType != null)
            { 
                findedEntry.EntryTypeId = entry.EntryTypeId;
                findedEntry.ImgName = entry.ImgName;
                findedEntry.Text = entry.Text;
                findedEntry.Title = entry.Title;
                findedEntry.TopicId = entry.TopicId;
                findedEntry.VideoName = entry.VideoName;

                db.Entries.Update(findedEntry);
                db.SaveChanges();
                return true;
            }

            return false;
        }

        internal bool DeleteEntry(int id)
        {
            var findedEntry = db.Entries.FirstOrDefault(e => e.Id == id);

            if (findedEntry != null)
            {
                db.Entries.Remove(findedEntry);
                db.SaveChanges();
                return true;
            }
            return false;
        }

        public bool AddEntry(EntryDTO entry)
        {
            var topic = db.Topics.FirstOrDefault(t => t.Id == entry.TopicId);
            var entryType = db.EntryTypes.FirstOrDefault(et => et.Id == entry.EntryTypeId);

            if (topic != null && entryType != null)
            {
                db.Entries.Add(new Entry() { EntryType = entryType, ImgName = entry.ImgName, Text = entry.Text, Title = entry.Title, Topic = topic, VideoName = entry.VideoName });
                db.SaveChanges();
                return true;
            }

            return false;
        }
    }
}
