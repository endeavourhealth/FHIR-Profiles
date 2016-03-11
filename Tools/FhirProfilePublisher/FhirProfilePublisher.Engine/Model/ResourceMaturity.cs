using FhirProfilePublisher.Specification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Engine
{
    internal enum ResourceMaturity : int
    {
        [Description("Initial development")]
        InitialDevelopment = 0,

        [Description("Draft (for review)")]
        Draft = 1,

        [Description("Ready for implementation")]
        ReadyForImplementation
    }

    internal static class ResourceMaturityHelper
    {
        public static string GetDescription(this ResourceMaturity resourceMaturity)
        {
            return EnumHelper.GetEnumDescription(resourceMaturity);
        }

        public static string GetAssociatedIcon(this ResourceMaturity resourceMaturity)
        {
            switch (resourceMaturity)
            {
                case ResourceMaturity.InitialDevelopment: return Images.IconBulletBlack;
                case ResourceMaturity.Draft: return Images.IconBulletOrange;
                case ResourceMaturity.ReadyForImplementation: return Images.IconBulletGreen;
            }

            return Images.IconBulletBlack;
        }
    }
}
