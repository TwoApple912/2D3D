using UnityEngine.UI;

namespace BlendModes
{
    [ExtendedComponent(typeof(Text))]
    public class UITextExtension : MaskableGraphicExtension<Text>
    {
        public override string[] GetSupportedShaderFamilies ()
        {
            return new[] {
                "UIDefaultFont"
            };
        }
    }
}
