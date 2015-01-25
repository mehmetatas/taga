﻿using System;

namespace Taga.Core.Validation.ValidationRules
{
    public class LessThanRule<T> : ComparisonRule<T> where T : IComparable<T>
    {
        public LessThanRule(T max)
            : base(max)
        {

        }

        protected override bool IsValid(int comparisonResult)
        {
            return comparisonResult == -1;
        }

        protected override bool InvalidParamResult
        {
            get { return false; }
        }
    }
}