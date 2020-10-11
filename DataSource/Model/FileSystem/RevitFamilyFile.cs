using DataSource.DataSource;
using DataSource.Model.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.System;

namespace DataSource.Models.FileSystem
{
    public class RevitFamilyFile : AFileNode, IMetadataContainer<Family>
    {
        public event EventHandler<EventArgs> MetadataUpdated;
        
        public const string FamilyExtension = "rfa";

        private readonly FamiliyMetadataContainer container; 

        private void OnMetadataUpdated()
        {
            MetadataUpdated?.Invoke(this, EventArgs.Empty);
        }

        public RevitFamilyFile()
        {
            container = new FamiliyMetadataContainer(this);
        }

        public override string FileExtension { get; } = FamilyExtension;

        public Family Metadata
        {
            get { return container.Metadata; }
        }

        public MetadataStatus Status
        {
            get { return container.Status; }
        }

        public bool AreMetadataValid
        {
            get { return container.AreMetadataValid; }
        }

        public bool AreMetadataRepairable
        {
            get { return container.AreMetadataRepairable; }
        }

        public bool HasFileMetadata
        {
            get { return container.HasFileMetadata; }
        }

        public bool HasEditMetadata
        {
            get { return container.HasEditMetadata; }
        }

        public bool IsBackup()
        {
            if (NameWithoutExtension.Contains(Constant.Point) == false) { return false; }

            var nameSplit = NameWithoutExtension.Split(Constant.PointChar);
            var backup = nameSplit.LastOrDefault();
            return string.IsNullOrWhiteSpace(backup) == false
                && backup.Length == 4
                && int.TryParse(backup, out _);
        }

        public IEnumerable<RevitFamilyFile> GetRevitBackups()
        {
            var search = new FileSearch<RevitFamilyFile> { Name = NameWithoutExtension, StartsWith = true };
            return Parent.GetFiles(false, search)
                         .Where(rvt => rvt.IsBackup());
        }

        public void DeleteBackups()
        {
            foreach (var file in GetRevitBackups())
            {
                file.Delete();
            }
        }

        public void Update()
        {
            var status = Status;
            container.Update();
            if(status != Status)
            {
                OnMetadataUpdated();
            }
        }

        public void Write(Family model)
        {
            container.Write(model);
        }

        public void SetApplicationDataSource()
        {
            container.SetApplicationDataSource();
        }

        public void SetExternalDataSource()
        {
            container.SetExternalDataSource();
        }

        public void SetExternalEditDataSource()
        {
            container.SetExternalEditDataSource();
        }
    }
}
