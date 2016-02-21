﻿using System;

namespace Data.Tests
{
    public class EMPBase
    {
        public DateTime HireDate { get; set; }
        public string EName { get; set; }
        public int EmpNo { get; set; }
        public int? Comm { get; set; }
    }

    public class EMP : EMPBase
    {
        public int? SAL { get; set; }       // Not used, because there are a Salary property tagged [Column("SAL")]
        [Apps72.Dev.Data.Annotations.Column("sal")]
        public int? Salary { get; set; }
        [Apps72.Dev.Data.Annotations.Column("MGR")]
        public int? Manager { get; set; }
        public int? MGR { get; set; }       // Not used, because there are a Manager property tagged [Column("MGR")]
        public string ColumnNotUse { get; set; }

        public static EMP Smith
        {
            get
            {
                return new EMP() { EmpNo = 7369, EName = "SMITH", HireDate = new DateTime(1980, 12, 17), Comm = null, Salary = 800 };
            }
        }
    }

}
