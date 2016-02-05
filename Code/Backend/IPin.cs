using System;

namespace Backend
{
	/// <summary>
	/// The Pin interface.
	/// </summary>
	public interface IPin
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		string Name{ get; set; }

		/// <summary>
		/// Gets or sets the display name.
		/// </summary>
		/// <value>The display name.</value>
		string DisplayName{ get; set; }

		/// <summary>
		/// Gets or sets the display number.
		/// </summary>
		/// <value>The display number.</value>
		string DisplayNumber { get; set; }

		/// <summary>
		/// Gets or sets the display number short.
		/// </summary>
		/// <value>The display number short.</value>
		string DisplayNumberShort { get; set; }

		/// <summary>
		/// Gets or sets the number.
		/// </summary>
		/// <value>The number.</value>
		uint Number{ get; set; }

		/// <summary>
		/// Gets or sets the hardware pin number/address.
		/// </summary>
		/// <value>The real number.</value>
		uint RealNumber { get; set; }

		/// <summary>
		/// Gets or sets the pin color.
		/// </summary>
		/// <value>The pin color </value>
		Gdk.Color PlotColor{ get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		Backend.PinType Type { get; set; }

		/// <summary>
		/// Gets or sets the mode.
		/// </summary>
		/// <value>The mode.</value>
		Backend.PinMode Mode { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PrototypeBackend.DPin"/> is SDA enabled.
		/// </summary>
		/// <value><c>true</c> if SDA enabled; otherwise, <c>false</c>.</value>
		bool SDA { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PrototypeBackend.DPin"/> is SCL enabled.
		/// </summary>
		/// <value><c>true</c> if SCL enabled; otherwise, <c>false</c>.</value>
		bool SCL{ get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PrototypeBackend.DPin"/> is RX enabled.
		/// </summary>
		/// <value><c>true</c> if RX enabled; otherwise, <c>false</c>.</value>
		bool RX{ get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PrototypeBackend.DPin"/> is TX enabled.
		/// </summary>
		/// <value><c>true</c> if TX enabled; otherwise, <c>false</c>.</value>
		bool TX{ get; set; }

		//Methods

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="PrototypeBackend.IPin"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="PrototypeBackend.IPin"/>.</returns>
		string ToString ();

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="PrototypeBackend.IPin"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="PrototypeBackend.IPin"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="PrototypeBackend.IPin"/>; otherwise, <c>false</c>.</returns>
		bool Equals (object obj);
	}
}

