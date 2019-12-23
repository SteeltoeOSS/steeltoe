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

using Microsoft.Extensions.Configuration;
using Steeltoe.Common;
using System.Collections.Generic;

namespace Steeltoe.Extensions.Configuration.CloudFoundry
{
    public class CloudFoundryApplicationOptions : ApplicationInstanceInfo
    {
        public static string PlatformConfigRoot => "vcap";

        public static readonly string ApplicationConfigRoot = PlatformConfigRoot + ":application";

        public CloudFoundryApplicationOptions()
        {
        }

        public CloudFoundryApplicationOptions(IConfiguration config)
            : base(config, PlatformConfigRoot)
        {
        }

        public string CF_Api { get; set; }

        public string Name { get; set; }

        public string Start { get; set; }

        public string Application_Id { get; set; }

        public override string ApplicationId => Application_Id;

        public string Application_Name { get; set; }

        public override string ApplicationName => Application_Name;

        public IEnumerable<string> Application_Uris { get; set; }

        public override IEnumerable<string> ApplicationUris => Application_Uris;

        public string Application_Version { get; set; }

        public override string ApplicationVersion => Application_Version;

        public string Instance_Id { get; set; }

        public override string InstanceId => Instance_Id;

        public int Instance_Index { get; set; } = -1;

        public override int InstanceIndex => Instance_Index;

        public string Space_Id { get; set; }

        public string SpaceId => Space_Id;

        public string Space_Name { get; set; }

        public string SpaceName => Space_Name;

        public string Instance_IP { get; set; }

        public override string InstanceIP => Instance_IP;

        public string Internal_IP { get; set; }

        public override string InternalIP => Internal_IP;

        public Limits Limits { get; set; }

        public override int DiskLimit => Limits == null ? -1 : Limits.Disk;

        public override int MemoryLimit => Limits == null ? -1 : Limits.Mem;

        public override int FileDescriptorLimit => Limits == null ? -1 : Limits.Fds;
    }
}
