/**
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

﻿using System.Runtime.InteropServices;

namespace DotPulsar.Internal
{
    [StructLayout(LayoutKind.Explicit)]
    public struct UIntUnion
    {
        public UIntUnion(byte b0, byte b1, byte b2, byte b3)
        {
            UInt = 0;
            B0 = b0;
            B1 = b1;
            B2 = b2;
            B3 = b3;
        }

        public UIntUnion(uint value)
        {
            B0 = 0;
            B1 = 0;
            B2 = 0;
            B3 = 0;
            UInt = value;
        }

        [FieldOffset(0)]
        public byte B0;
        [FieldOffset(1)]
        public byte B1;
        [FieldOffset(2)]
        public byte B2;
        [FieldOffset(3)]
        public byte B3;

        [FieldOffset(0)]
        public uint UInt;
    }
}
