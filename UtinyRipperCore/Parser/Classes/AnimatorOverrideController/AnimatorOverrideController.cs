﻿using System.Collections.Generic;
using UtinyRipper.AssetExporters;
using UtinyRipper.Classes.AnimatorOverrideControllers;
using UtinyRipper.Exporter.YAML;
using UtinyRipper.SerializedFiles;

namespace UtinyRipper.Classes
{
	public sealed class AnimatorOverrideController : RuntimeAnimatorController
	{
		public AnimatorOverrideController(AssetInfo assetInfo) :
			base(assetInfo)
		{
		}

		public override void Read(AssetStream stream)
		{
			base.Read(stream);

			Controller.Read(stream);
			m_clips = stream.ReadArray<AnimationClipOverride>();
		}

		public override IEnumerable<Object> FetchDependencies(ISerializedFile file, bool isLog = false)
		{
			foreach(Object @object in base.FetchDependencies(file, isLog))
			{
				yield return @object;
			}
			
			yield return Controller.FetchDependency(file, isLog, ToLogString, "m_Controller");
			foreach (AnimationClipOverride clip in Clips)
			{
				foreach (Object @object in clip.FetchDependencies(file, isLog))
				{
					yield return @object;
				}
			}
		}

		protected override YAMLMappingNode ExportYAMLRoot(IAssetsExporter exporter)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(exporter);
			node.Add("m_Controller", Controller.ExportYAML(exporter));
			node.Add("m_Clips", Clips.ExportYAML(exporter));
			return node;
		}

		public override string ExportExtension => "overrideController";

		public IReadOnlyList<AnimationClipOverride> Clips => m_clips;

		public PPtr<RuntimeAnimatorController> Controller;

		private AnimationClipOverride[] m_clips;
	}
}
