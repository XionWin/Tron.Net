using System;

namespace Tron.Drawing
{
	public struct SizeI
	{
		//
		// Summary:
		//     Represents a new instance of the Gaea.Drawing.Size class with member data left
		//     uninitialized.
		public static readonly SizeI Empty;

		public SizeI(int width, int height)
		{
			this.Width = width;
			this.Height = height;
		}

		//
		// Summary:
		//     Gets a value that indicates whether this SkiaSharp.SKSize structure has zero
		//     width and height.
		public bool IsEmpty
		{
			get => this.Width == int.MinValue || this.Height == int.MinValue;
		}
		//
		// Summary:
		//     Gets or sets the horizontal component of this SkiaSharp.SKSize structure.
		public int Width { get; set; }
		//
		// Summary:
		//     Gets or sets the vertical component of this SkiaSharp.SKSize structure.
		public int Height { get; set; }

		//
		// Summary:
		//     Adds the width and height of one SkiaSharp.SKSize structure to the width and
		//     height of another SkiaSharp.SKSize structure.
		//
		// Parameters:
		//   sz1:
		//     The first SkiaSharp.SKSize structure to add.
		//
		//   sz2:
		//     The second SkiaSharp.SKSize structure to add.
		//
		// Returns:
		//     A SkiaSharp.SKSize structure that is the result of the addition operation.
		public static SizeI Add(SizeI sz1, SizeI sz2)
		{
			return new SizeI(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		}
		//
		// Summary:
		//     Subtracts the width and height of one SkiaSharp.SKSize structure from the width
		//     and height of another SkiaSharp.SKSize structure.
		//
		// Parameters:
		//   sz1:
		//     The SkiaSharp.SKSize structure on the left side of the subtraction operator.
		//
		//   sz2:
		//     The SkiaSharp.SKSize structure on the right side of the subtraction operator.
		//
		// Returns:
		//     A SkiaSharp.SKSize that is the result of the subtraction operation.
		public static SizeI Subtract(SizeI sz1, SizeI sz2)
		{
			return new SizeI(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		}
		//
		// Summary:
		//     Tests to see whether the specified object is a SkiaSharp.SKSize structure with
		//     the same dimensions as this SkiaSharp.SKSize structure.
		//
		// Parameters:
		//   obj:
		//     The System.Object to test.
		//
		// Returns:
		//     This method returns true if obj is a SkiaSharp.SKSize and has the same coordinates
		//     as this SkiaSharp.SKSize.
		public override bool Equals(object obj)
		{
			SizeI size = (SizeI)obj;
			return size.Width == this.Width && size.Height == this.Height;
		}
		//
		// Summary:
		//     Returns a hash code for this SkiaSharp.SKSize structure.
		//
		// Returns:
		//     An integer value that specifies a hash value for this SkiaSharp.SKSize structure.
		public override int GetHashCode()
		{
			return this.Width.GetHashCode();
		}
		//
		// Summary:
		//     Converts a SkiaSharp.SKSize structure to a SkiaSharp.SKSizeI structure.
		//
		// Returns:
		//     Returns a SkiaSharp.SKSizeI structure.
		public Size ToSize()
		{
			return new Size(this.Width, this.Height);
		}
		//
		// Summary:
		//     Converts this SkiaSharp.SKSize to a human readable string.
		//
		// Returns:
		//     A string that represents this SkiaSharp.SKSize.
		public override string ToString()
		{
			return string.Format(@"\{{0}, {1}\}", this.Width, this.Height);
		}

		//
		// Summary:
		//     Adds the width and height of one SkiaSharp.SKSize structure to the width and
		//     height of another SkiaSharp.SKSize structure.
		//
		// Parameters:
		//   sz1:
		//     The first SkiaSharp.SKSize structure to add.
		//
		//   sz2:
		//     The second SkiaSharp.SKSize structure to add.
		//
		// Returns:
		//     A SkiaSharp.SKSize structure that is the result of the addition operation.
		public static SizeI operator +(SizeI sz1, SizeI sz2)
		{
			return Add(sz1, sz2);
		}
		//
		// Summary:
		//     Subtracts the width and height of one SkiaSharp.SKSize structure from the width
		//     and height of another SkiaSharp.SKSize structure.
		//
		// Parameters:
		//   sz1:
		//     The SkiaSharp.SKSize structure on the left side of the subtraction operator.
		//
		//   sz2:
		//     The SkiaSharp.SKSize structure on the right side of the subtraction operator.
		//
		// Returns:
		//     A SkiaSharp.SKSize that is the result of the subtraction operation.
		public static SizeI operator -(SizeI sz1, SizeI sz2)
		{
			return Subtract(sz1, sz2);
		}
		//
		// Summary:
		//     Tests whether two SkiaSharp.SKSize structures are equal.
		//
		// Parameters:
		//   sz1:
		//     The SkiaSharp.SKSize structure on the left side of the equality operator.
		//
		//   sz2:
		//     The SkiaSharp.SKSize structure on the right of the equality operator.
		//
		// Returns:
		//     This operator returns true if both SkiaSharp.SKSize structures have equal SkiaSharp.SKSize.Width
		//     and SkiaSharp.SKSize.Height; otherwise, false.
		public static bool operator ==(SizeI sz1, SizeI sz2)
		{
			return sz1.Equals(sz2);
		}
		//
		// Summary:
		//     Tests whether two SkiaSharp.SKSize structures are different.
		//
		// Parameters:
		//   sz1:
		//     The SkiaSharp.SKSize structure that is to the left of the inequality operator.
		//
		//   sz2:
		//     The SkiaSharp.SKSize structure that is to the right of the inequality operator.
		//
		// Returns:
		//     This operator returns true if either of the SkiaSharp.SKSize.Width and SkiaSharp.SKSize.Height
		//     properties of the two SkiaSharp.SKSize structures are unequal; otherwise false.
		public static bool operator !=(SizeI sz1, SizeI sz2)
		{
			return !sz1.Equals(sz2);
		}

		//
		// Summary:
		//     Converts the specified SkiaSharp.SKSizeI structure to a SkiaSharp.SKSize structure.
		//
		// Parameters:
		//   size:
		//     The SkiaSharp.SKSizeI structure to be converted.
		//
		// Returns:
		//     The SkiaSharp.SKSize structure structure to which this operator converts.
		public static implicit operator SizeI(Size size)
		{
			return size.ToSizeI();
		}
		
	}
}
