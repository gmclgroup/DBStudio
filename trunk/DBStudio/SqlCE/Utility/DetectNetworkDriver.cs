//=============================================================================
//    DBStudio
//    Copyright (C) 2006  ms44

//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation; either
//    version 2 of the License, or (at your option) any later version.

//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.

//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

//    If you have any questions ,please contact me via 54715112@qq.com
//===============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;

namespace DBStudio.Utility
{

    internal class DetectNetworkDriver
    {
        /// <summary>
        /// Win32 P/Invoke to GetDriveType
        /// </summary>
        /// <param name="pathName">Drive specification including the colon - i.e. C:, K:, Z:, etc.</param>
        /// <returns>An UInt32 value identifying the drive type. Actual #define values are defined in
        /// WinBase.h - the values returned by this function correspond directly to the .NET DriveType enumeration</returns>
        [DllImport("Kernel32.dll")]
        static extern UInt32 GetDriveType(string pathName);

        /// <summary>
        /// Determines whether a file path is located on a Network Share
        /// </summary>
        /// <param name="pathName">Absolute or relative pathname of the file.
        /// The pathname is not required to currently exist</param>
        /// <returns>True if file path is located on a Network Share</returns>
        internal static bool IsPathOnNetworkDrive(string pathName)
        {
            bool result=false;
            DriveType dType=GetDriveTypeForPath(pathName);
            if(dType== DriveType.Network)
            {
                result=true;
            }
            if (dType == DriveType.NoRootDirectory)
            {
                result = true;
            }

            return result;
        }

        ///// <summary>
        ///// Identify a file path's drive type (local disk, network share, removable drive, etc.)
        ///// </summary>
        ///// <param name="pathName">Absolute or relative pathname of the file
        ///// The pathname is not required to currently exist</param>
        ///// <returns>A DriveType enumeration value identifying the device type where the file path is located</returns>
        //private static DriveType GetDriveTypeForPath(string pathName)
        //{
        //    DriveType driveType = DriveType.Unknown;
        //    string driveName = GetDriveNameForPath(pathName);

        //    string driveObjectname = string.Format("Win32_LogicalDisk.DeviceID=\"{0}\"", driveName);
        //    ManagementObject driveObj = new ManagementObject(driveObjectname);
        //    driveObj.Get();
        //    PropertyData driveTypePropertyData = driveObj.Properties["DriveType"];
        //    if (driveTypePropertyData.Value != null)
        //        driveType = (DriveType)Convert.ToInt32(driveTypePropertyData.Value);

        //    return driveType;
        //}

        /// <summary>
        /// Identifies the drive portion of a file path
        /// </summary>
        /// <param name="pathName">Absolute or relative pathname of the file
        /// The pathname is not required to currently exist</param>
        /// <returns>A string containing the drive portion of the file path. The drive string contains the colon
        /// i.e. C:, K:, Z:, etc.</returns>
        private static string GetDriveNameForPath(string pathName)
        {
            string pathRoot = Directory.GetDirectoryRoot(pathName);
            int idxSeperator = pathRoot.IndexOf(Path.VolumeSeparatorChar);
            string driveName = pathRoot.Substring(0, idxSeperator + 1);

            return driveName;
        }



        /// <summary>
        /// Identify the type of drive (local disk, network share, removable drive, etc.) a file path
        /// is located on.
        /// </summary>
        /// <remarks>This function uses P/Invoke to call out to a Win32 API function and therefore may introduce
        /// significant Code Access Security Concerns</remarks>
        /// <param name="pathName">Absolute or relative pathname of the file
        /// The pathname is not required to currently exist</param>
        /// <returns>A DriveType enumeration value identifying the device type where the file path is located</returns>
        static DriveType GetDriveTypeForPath(string pathName)
        {
            string driveName = GetDriveNameForPath(pathName);
            return (DriveType)Convert.ToInt32(GetDriveType(driveName));
        }
    }
}
