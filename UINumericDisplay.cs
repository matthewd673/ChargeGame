using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Verdant;
using Verdant.UI;

namespace ChargeGame
{
	public class UINumericDisplay : UIElement
	{

		public SpriteSheet FontSheet { get; private set; }
		public int FontSheetOffset { get; set; } = 0;

		private string _valueString = "";
		private long _value;
		public long Value
		{
			get { return _value; }
			set
			{
				_value = value;
				_valueString = _value.ToString();

				BoxModel.Width = FontSheet.Width * _valueString.Length;
			}
		}

		public UINumericDisplay(Vec2 position, SpriteSheet fontSheet)
			: base(position, 0, fontSheet.Height)
		{
			FontSheet = fontSheet;
		}

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

			// manually draw digits
			int drawOffset = 0;
			foreach (char c in _valueString.ToCharArray())
			{
				FontSheet.DrawIndex(spriteBatch,
									new Rectangle(
										(int)(AbsoluteContentPosition.X + drawOffset) * Renderer.UIScale,
										(int)AbsoluteContentPosition.Y * Renderer.UIScale,
										FontSheet.Width * Renderer.UIScale,
										FontSheet.Height * Renderer.UIScale
										),
									c - 48 + FontSheetOffset
									);
				drawOffset += FontSheet.Width;
			}

			//base.DrawBounds(spriteBatch);
		}
    }
}

