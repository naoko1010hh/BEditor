﻿using System.Collections.Generic;

using BEditorCore.Data.EffectData;
using BEditorCore.Data.ObjectData;

namespace BEditorCore.Data.ProjectData {
    public class ObjectLoadArgs {

        public ObjectLoadArgs(int frame, List<ClipData> schedules) {
            Frame = frame;
            Schedules = schedules;
        }

        public int Frame { get; }
        public List<ClipData> Schedules { get; }
        public bool Handled { get; set; }
    }

    public class EffectLoadArgs {

        public EffectLoadArgs(int frame, List<EffectElement> schedules) {
            Frame = frame;
            Schedules = schedules;
        }

        public int Frame { get; }
        public List<EffectElement> Schedules { get; }
        public bool Handled { get; set; }
    }
}