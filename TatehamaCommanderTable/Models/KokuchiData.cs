﻿using System;

namespace TatehamaCommanderTable.Models
{
    /// <summary>
    /// 運転告知器データクラス
    /// </summary>
    public class KokuchiData
    {
        public KokuchiType Type { get; set; }
        public string DisplayData { get; set; }
        public DateTime OriginTime { get; set; }

        public KokuchiData(KokuchiType Type, string DisplayData, DateTime OriginTime)
        {
            this.Type = Type;
            this.DisplayData = DisplayData;
            this.OriginTime = OriginTime;
        }
    }

    public enum KokuchiType
    {
        None,
        Yokushi,
        Tsuuchi,
        TsuuchiKaijo,
        Kaijo,
        Shuppatsu,
        ShuppatsuJikoku,
        Torikeshi,
        Tenmatsusho
    }
}
