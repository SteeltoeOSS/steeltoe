﻿#pragma warning disable SA1636 // File header copyright text should match

// <copyright file="Int64MeasureMinMaxSumCountAggregator.cs" company="OpenTelemetry Authors">
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

using Steeltoe.Management.OpenTelemetry.Metrics.Export;
using System;
using System.Threading;

namespace Steeltoe.Management.OpenTelemetry.Metrics.Aggregators
{
    /// <summary>
    /// Aggregator which calculates summary (Min,Max,Sum,Count) from measures.
    /// </summary>
    [Obsolete("OpenTelemetry Metrics API is not considered stable yet, see https://github.com/SteeltoeOSS/Steeltoe/issues/711 more information")]
    internal class Int64MeasureMinMaxSumCountAggregator : Aggregator<long>
    {
        private LongSummary _summary = new ();
        private LongSummary _checkPoint = new ();
        private object _updateLock = new ();

        public override void Checkpoint()
        {
            _checkPoint = Interlocked.Exchange(ref _summary, new LongSummary());
        }

        public override AggregationType GetAggregationType() => AggregationType.Summary;

        public override MetricData<long> ToMetricData()
        {
            return new SummaryData<long>
            {
                Count = _checkPoint.Count,
                Sum = _checkPoint.Sum,
                Min = _checkPoint.Min,
                Max = _checkPoint.Max,
                Timestamp = DateTime.UtcNow,
            };
        }

        public override void Update(long value)
        {
            lock (_updateLock)
            {
                _summary.Count++;
                _summary.Sum += value;
                _summary.Max = Math.Max(_summary.Max, value);
                _summary.Min = Math.Min(_summary.Min, value);
            }
        }

        private class LongSummary
        {
            public long Count;
            public long Min;
            public long Max;
            public long Sum;

            public LongSummary()
            {
                Min = long.MaxValue;
                Max = long.MinValue;
            }
        }
    }
}
