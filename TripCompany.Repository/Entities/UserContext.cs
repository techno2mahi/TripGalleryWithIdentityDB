using Microsoft.Owin.FileSystems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripCompany.Repository.Entities
{
    public class UserContext : IDisposable
    {
        private string _fileDBLocation;

        public UserContext(string fileDBLocation)
        {
            _fileDBLocation = fileDBLocation;
 
            var fileSystem = new Microsoft.Owin.FileSystems.PhysicalFileSystem("");

            IFileInfo fi;
            if (fileSystem.TryGetFileInfo(_fileDBLocation, out fi))
            {

                var json = File.ReadAllText(fi.PhysicalPath);
                var result = JsonConvert.DeserializeObject<List<User>>(json);

                Users = result.ToList();
            }
        }

        public IList<User> Users { get; set; }

        public bool SaveChanges()
        {
            // write trips to json file, overwriting the old one

            var json = JsonConvert.SerializeObject(Users);

            var fileSystem = new Microsoft.Owin.FileSystems.PhysicalFileSystem("");

            IFileInfo fi;
            if (fileSystem.TryGetFileInfo(_fileDBLocation, out fi))
            {
                File.WriteAllText(fi.PhysicalPath, json);
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            // cleanup
        }
    }
}
