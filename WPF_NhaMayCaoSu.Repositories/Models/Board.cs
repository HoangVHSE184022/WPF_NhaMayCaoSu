﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_NhaMayCaoSu.Repository.Models
{
    public class Board
    {
        public Guid BoardId {  get; set; }
        public string BoardName { get; set; }
        public string BoardIp { get; set;}
        public string BoardMacAddress { get; set; }
        public int BoardMode { get; set;}
    }
}