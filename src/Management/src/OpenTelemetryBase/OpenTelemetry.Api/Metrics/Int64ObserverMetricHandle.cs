﻿#pragma warning disable SA1636 // File header copyright text should match

// <copyright file="Int64ObserverMetricHandle.cs" company="OpenTelemetry Authors">
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

namespace Steeltoe.Management.OpenTelemetry.Metrics
{
    /// <summary>
    /// Handle to the metrics observer with the defined <see cref="LabelSet"/>.
    /// </summary>
    internal abstract class Int64ObserverMetricHandle
    {
        /// <summary>
        /// Observes the given value.
        /// </summary>
        /// <param name="value">value by which the observer handle should be Recorded.</param>
        public abstract void Observe(long value);
    }
}
