using DataSource.Metadata;
using DataSource.Models;
using DataSource.Models.FileSystem;
using System;
using System.Diagnostics.CodeAnalysis;
using DataSource.Model.Metadata;

namespace DataSource.DataSource.Json
{
    public class FamilyJsonDataSource : JsonDataSource<Family>, IMetadataDataSource<Family, RevitFamilyFile>
    {
        private readonly JsonDataSource<Family> dataSource = new JsonDataSource<Family>();

        public override Family Read()
        {
            return dataSource.Read();
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
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

        public virtual void SetFile(RevitFamilyFile fileNode)
        {
            if (fileNode is null) { throw new ArgumentNullException(nameof(fileNode)); }

            var jsonFile = fileNode.ChangeExtension<JsonFile>();
            dataSource.SetFile(jsonFile);
        }
    }
}