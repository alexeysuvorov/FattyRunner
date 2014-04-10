﻿using System;

namespace FattyRunner.Interfaces {
    [AttributeUsage(AttributeTargets.Method)]
    public class FatTestAttribute : Attribute {
        public FatTestAttribute(uint maxIterations = 1000u,
                                uint warmUpIterations = 50,
                                uint step = 100) {

            MaxIterations = maxIterations;
            WarmUpIterations = warmUpIterations;
            Step = step;
        }
        public uint MaxIterations { get; set; }
        public uint WarmUpIterations { get; set; }
        public uint Step { get; set; }
    }
}