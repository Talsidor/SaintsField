// using MarkupAttributes;

using UnityEngine;

namespace SaintsField.Samples.Scripts
{
    public class CompatibilityNaAndDefault : MonoBehaviour
    {
        [RichLabel("<color=green>+Native</color>"), Range(0, 5)]
        public float nativeRange;
        // this wont work. Please put `SaintsField` before other drawers
        [Range(0, 5), RichLabel("<color=green>+Native</color>")]
        public float nativeRangeHandled;

#if SAINTSFIELD_SAMPLE_NAUGHYTATTRIBUTES
        // this works
        [RichLabel("<color=green>+NA</color>"), NaughtyAttributes.Label(" "), NaughtyAttributes.CurveRange(0, 0, 1, 1, NaughtyAttributes.EColor.Green)]
        public AnimationCurve naCurve;

        // this wont work too. Please put `SaintsField` before other drawers
        [NaughtyAttributes.CurveRange(0, 0, 1, 1, NaughtyAttributes.EColor.Green), RichLabel("<color=green>+NA</color>")]
        public AnimationCurve naCurveHandled;

        [RichLabel("<color=green>+NA</color>"), NaughtyAttributes.ProgressBar(100)]
        public int progressBar;

        [RichLabel("<color=green>+NA</color>"), NaughtyAttributes.InputAxis]
        public string inputAxis;
#endif
        //
        // [Tooltip("TT1")]
        // public int tt1;
        //
        // [RichLabel("Rich"), Tooltip("TT2"), MinMaxSlider(0, 10)] public Vector2 tt2;
        //
        // // [TabScope("Tab Scope", "Left|Right", box: true)]
        // // [Tab("./Left")]
        // // [MinValue(10), AboveRichLabel("Test Cap")]
        // // public int one;
        // //
        // // [Tab("../Right")]
        // // [MinMaxSlider(0, nameof(one)), RichLabel("MinMaxSlider")]
        // // public Vector2 two;
        // // public int three;
        //
        // // [Box("Group")]
        // // public int one;
        // // [TitleGroup("Group/Nested Group 1")]
        // // public int two;
        // // public int three;
        // // [TitleGroup("Group/Nested Group 2")]
        // // public int four;
        // // public int five;
    }
}
