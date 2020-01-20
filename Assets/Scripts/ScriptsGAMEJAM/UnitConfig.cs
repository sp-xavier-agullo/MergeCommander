using UnityEngine;
using UnityEngine.Serialization;

namespace GJ18
{
	[CreateAssetMenu(fileName = "UnitConfig", menuName = "SPGameJam18/UnitConfig", order = 1)]
	public class UnitConfig : ScriptableObject
	{
		public BoardObjectType type;
		public Category category;

		public Sprite sprite;
		public Sprite stickerSprite;
		public Sprite stickerSpriteDark;

		public string unitLongName;
		public string unitShortName;
		public string unitDesc;

		public int unitPower;
		public int unitNumAttacks;
		public float attackChance;
		public float defenseChance;

		public int availableFromLevel;
		
		public string unitRanged;
		public string unitFlies;

		public Sprite recipeItem1;
		public Sprite recipeItem2;
		public Sprite recipeItem3;

		public Sprite portraitBig;
		public Sprite portraitButton;
		public Sprite portraitButtonLocked;

	}
}