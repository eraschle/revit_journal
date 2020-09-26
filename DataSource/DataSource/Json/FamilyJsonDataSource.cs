using DataSource.Metadata;
using DataSource.Model;
using DataSource.Model.Family;
using DataSource.Model.FileSystem;
using System;

namespace DataSource.DataSource.Json
{
    public class FamilyJsonDataSource : JsonDataSource<Family>, IMetadataDataSource
    {
        private JsonDataSource<Family> dataSource = new JsonDataSource<Family>();

        public override Family Read()
        {
            return dataSource.Read();
        }

        public MetadataStatus UpdateStatus()
        {
            var status = MetadataStatus.Valid;
            try
            {
                if (FileNode is null
                    || FileNode.Exists() == false
                    || Read() is null)
                {
                    status = MetadataStatus.Error;
                }
            }
            catch
            {
                status = MetadataStatus.Error;
            }

            return status;
        }

        public override void Write(Family model)
        {
            dataSource.Write(model);
        }

        public virtual void SetFamilyFile(RevitFamilyFile fileNode)
        {
            if(fileNode is null) { throw new ArgumentNullException(nameof(fileNode)); }

            var jsonFile = fileNode.ChangeExtension<JsonFile>();
            dataSource.SetFile(jsonFile);
        }
    }
}