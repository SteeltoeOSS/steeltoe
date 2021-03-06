﻿#pragma warning disable SA1636 // File header copyright text should match

// <copyright file="Int64CounterMetricSdk.cs" company="OpenTelemetry Authors">
// Copyright 2018, OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
#pragma warning restore SA1636 // File header copyright text should match

using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;

namespace Steeltoe.Management.OpenTelemetry.Metrics
{
    [Obsolete("OpenTelemetry Metrics API is not considered stable yet, see https://github.com/SteeltoeOSS/Steeltoe/issues/711 more information")]
    internal class Int64CounterMetricSdk : CounterMetricSdkBase<long>
    {
        public Int64CounterMetricSdk(string name)
            : base(name)
        {
        }

        public override void Add(in SpanContext context, long value, LabelSet labelset)
        {
            // user not using bound instrument. Hence create a  short-lived bound instrument.
            Bind(labelset, isShortLived: true).Add(context, value);
        }

        public override void Add(in SpanContext context, long value, IEnumerable<KeyValuePair<string, string>> labels)
        {
            // user not using bound instrument. Hence create a short-lived bound instrument.
            Bind(new LabelSetSdk(labels), isShortLived: true).Add(context, value);
        }

        protected override BoundCounterMetricSdkBase<long> CreateMetric(RecordStatus recordStatus) => new Int64BoundCounterMetricSdk(recordStatus);
    }
}
