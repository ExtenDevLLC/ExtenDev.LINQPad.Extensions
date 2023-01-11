using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ExtenDev.LINQPad.Extensions.UI;
using LINQPad;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;

namespace ExtenDev.LINQPad.Extensions.Development
{
    public static class MSBuildExtensions
    {

        public static object ToDumpable(this ProjectProperty property)
        {
            if (property == null) return null;

            return new
            {
                property.Name,
                property.UnevaluatedValue,
                property.EvaluatedValue,
                Predecessor = property.Predecessor?.Transform(pred => Util.OnDemand(pred.GetType().Name, () => pred.ToDumpable())),
                FromFile = property.Xml?.Transform(xml => new Hyperlinq(() => OpenWithBox.Show(xml.ContainingProject.FullPath, xml.Location.Line), Path.GetFileName(xml.ContainingProject.FullPath))),
                OriginalObject = Util.OnDemand(property.GetType().Name, () => property)
            };
        }

        public static object ToDumpable(this ProjectTargetInstance target)
        {
            return new
            {
                target.Name,
                target.BeforeTargets,
                target.AfterTargets,
                target.DependsOnTargets,
                target.Condition,
                target.Inputs,
                target.Returns,
                FromFile = target.FullPath?.Transform(path => new Hyperlinq(() => OpenWithBox.Show(path, target.Location.Line), Path.GetFileName(path))),
                OriginalObject = Util.OnDemand(target.GetType().Name, () => target)
            };
        }

        public static object ToDumpable(this ProjectItem item)
        {
            return new
            {
                item.ItemType,
                item.EvaluatedInclude,
                item.UnevaluatedInclude,
                Metadata = Util.OnDemand($"{item.Metadata.Count} Values", () => item.Metadata.Select(m => m.ToDumpable())),
                // Seems like these are now always the same and that DirectMetadata may not actually exist in some version of the library?
                //DirectMetadata = Util.OnDemand($"{item.DirectMetadata.Count()} Values", () => item.DirectMetadata.Select(m => m.ToDumpable())),
                Xml = item.Xml?.Transform(xml => new Hyperlinq(() => OpenWithBox.Show(xml.ContainingProject.FullPath, xml.Location.Line), Path.GetFileName(xml.ContainingProject.FullPath))),
            };
        }

        public static object ToDumpable(this ProjectMetadata metadata)
        {
            return new
            {
                metadata.Name,
                metadata.EvaluatedValue,
                metadata.UnevaluatedValue,
                metadata.IsImported,
                Predecessor = metadata.Predecessor?.Transform(pred => Util.OnDemand(pred.GetType().Name, () => pred.ToDumpable())),
                Xml = metadata.Xml?.Transform(xml => new Hyperlinq(() => OpenWithBox.Show(xml.ContainingProject.FullPath, xml.Location.Line), Path.GetFileName(xml.ContainingProject.FullPath))),
            };
        }

        public static object ToDumpable(this ProjectItemDefinition itemDef)
        {
            return new
            {
                itemDef.ItemType,
                Metadata = Util.OnDemand($"{itemDef.Metadata.Count()} Elements", () => itemDef.Metadata.Select(m => m.ToDumpable())),
            };
        }

    }
}
