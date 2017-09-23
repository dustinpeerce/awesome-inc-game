using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameVals {

    public class Settings
    {
        public static string musicVolumeKey = "Music Volume";
        public static string audioClipVolumeKey = "Audio Clip Volume";
    }

    public class HubCanvas
    {
        public static string hubGameBeginPanelHeader;
        public static string hubGameBeginPanelBody;

        public static string hubCoreValueOneBody;
        public static string hubCoreValueTwoBody;
        public static string hubCoreValueThreeBody;
        public static string hubCoreValueFourBody;

        public static string hubGameEndPanelHeader;
        public static string hubGameEndPanelBody;

        public static string hubGameAdvancedPanelHeader;
        public static string hubGameAdvancedPanelBody;
        public static string hubGameAdvancedPanelFooter;
    }

    public static int defaultLayerIndex = 0;
    public static int droppingLayerIndex = 8;
    public static int groundLayerIndex = 9;
}
