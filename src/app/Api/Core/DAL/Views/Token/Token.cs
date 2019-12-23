﻿using System;
using Api.Core.Enums;

namespace Api.Core.DAL.Views.Token
{
    public class Token : BaseView
    {
        public TokenTypeEnum Type { get; set; }
        public string Value { get; set; }
        public DateTime ExpireAt { get; set; }
        public string UserId { get; set; }
    }
}