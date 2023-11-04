﻿using UnityEngine;

namespace SaintsField.Samples
{
    public class SepTitleExample: MonoBehaviour
    {
        [SepTitle("Separate Here", EColor.Pink)]
        public string content1;

        [SepTitle(EColor.Green)]
        public string content2;
    }
}
