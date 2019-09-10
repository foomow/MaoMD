﻿using System;
namespace Demo
{
    /// <summary>
    /// Demo Class
    /// </summary>
    public class DemoClass
    {
        private int _id;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="id">id 初始化值</param>
        public DemoClass(int id)
        {
            _id = id;
        }
        /// <summary>
        /// property Id
        /// </summary>
        public int Id { get => _id; set=>_id=value; }
    }
    /// <summary>
    /// an enum type
    /// </summary>
    enum WeekDay {
        /// <summary>
        /// sunday
        /// </summary>
        Sun,
        /// <summary>
        /// monday
        /// </summary>
        Mon,
        /// <summary>
        /// tuesday
        /// </summary>
        Tue,
        /// <summary>
        /// wedensday
        /// </summary>
        Wed,
        /// <summary>
        /// thurseday
        /// </summary>
        Thu,
        /// <summary>
        /// friday
        /// </summary>
        Fri,
        /// <summary>
        /// saturday
        /// </summary>
        Sat
    }
}
