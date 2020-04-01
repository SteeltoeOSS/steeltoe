﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using OpenTelemetry.Metrics.Export;
using Steeltoe.Management.OpenTelemetry.Metrics.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Steeltoe.Management.OpenTelemetry.Metrics.Exporter
{
    public class MultiExporter : MetricExporter
    {
        private readonly IEnumerable<MetricExporter> _exporters;

        public MultiExporter(IEnumerable<MetricExporter> exporters)
        {
            this._exporters = exporters;
        }

        public override async Task<ExportResult> ExportAsync<T>(List<Metric<T>> metrics, CancellationToken cancellationToken)
        {
            foreach (var exporter in _exporters)
            {
                await exporter.ExportAsync(metrics, cancellationToken);
            }

            return ExportResult.Success;
        }

    }
}