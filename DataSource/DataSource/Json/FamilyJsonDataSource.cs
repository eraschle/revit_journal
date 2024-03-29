﻿using DataSource.DataSource.Json;
using DataSource.Metadata;
using DataSource.Model.Family;
using DataSource.Model.FileSystem;

namespace DataSource.Json
{
    public class MetadataJsonDataSource : AMetadataDataSource<Family>
    {
        public JsonDataSource<Family> JsonDataSource { get; private set; }

        public MetadataJsonDataSource(RevitFile revitFile) : base(revitFile)
        {
            JsonDataSource = new JsonDataSource<Family>(revitFile);
        }

        public override void AddFileNameSuffix(params string[] suffixes)
        {
            JsonDataSource.AddFileNameSuffix(suffixes);
        }

        public override bool Exist { get { return JsonDataSource.JsonFile.Exist; } }

        public override Family Read(AFile source = null)
        {
            JsonFile jsonFile = null;
            if(source is JsonFile json)
            {
                jsonFile = json;
            }
            return JsonDataSource.Read(jsonFile);
        }

        public override void UpdateStatus()
        {
            if (JsonDataSource.JsonFile.Exist == false)
            {
                Status = MetadataStatus.Error;
            }
            else
            {
                try
                {
                    if (Read() != null)
                    {
                        Status = MetadataStatus.Valid;
                    }
                    else
                    {
                        Status = MetadataStatus.Error;
                    }
                }
                catch
                {
                    Status = MetadataStatus.Error;
                }
            }
        }

        public override void Write(Family model, AFile destination = null)
        {
            if (destination is null)
            {
                JsonDataSource.Write(model);
            }
            else if(destination is JsonFile jsonFile)
            {
                JsonDataSource.Write(model, jsonFile);
            }
        }
    }
}