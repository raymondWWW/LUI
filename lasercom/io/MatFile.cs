﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HDF5DotNet;

namespace lasercom.io
{
    public class MatFile:IDisposable
    {
        private readonly string _FileName;
        public string FileName
        {
            get
            {
                return _FileName;
            }
        }
        private readonly H5FileId FileId;
        private readonly H5GroupId GroupId;

        Dictionary<string, MatVar> Variables;

        public MatFile(string fileName)
        {
            _FileName = fileName;
            FileId = H5F.create(FileName, H5F.CreateMode.ACC_TRUNC);
            GroupId = H5G.open(FileId, "/");

            Variables = new Dictionary<string, MatVar>();
        }

        public MatVar<T> CreateVariable<T>(string Name, params long[] Dims)
        {
            MatVar<T> V = new MatVar<T>(Name, GroupId, Dims);
            Variables.Add(Name, V);
            return V;
        }

        private void PrependMatlabHeader(string filename)
        {
            byte[] header = new byte[512];

            string headerText = "MATLAB 7.3 MAT-file";
            byte[] headerTextBytes = Encoding.ASCII.GetBytes(headerText);

            for (int i = 0; i < headerText.Length; i++) header[i] = headerTextBytes[i];

            header[124] = 0;
            header[125] = 2;
            header[126] = Encoding.ASCII.GetBytes("I")[0];
            header[127] = Encoding.ASCII.GetBytes("M")[0];

            string tempfile = Path.GetTempFileName();

            using (var newFile = new FileStream(tempfile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                newFile.Write(header, 0, header.Length);

                using (var oldFile = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    oldFile.CopyTo(newFile);
                }
            }
            File.Copy(tempfile, filename, true);
        }

        public void Close()
        {
            foreach (var V in Variables.Values) V.Dispose();
            H5G.close(GroupId);
            H5F.close(FileId);
            PrependMatlabHeader(_FileName);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
