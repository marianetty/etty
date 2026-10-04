#region Using declarations
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Data;
using NinjaTrader.Gui;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

// Bull Market Support Band (BMSB) para NinjaTrader 8.
// SMA de 20 semanas + EMA de 21 semanas calculadas siempre sobre velas
// semanales, aunque el gráfico esté en otro timeframe (1h, 4h, diario...).
// Precio por encima de la banda = régimen alcista; por debajo = bajista.
namespace NinjaTrader.NinjaScript.Indicators
{
	public class BullMarketSupportBand : Indicator
	{
		private SMA weeklySma;
		private EMA weeklyEma;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description					= "Bull Market Support Band: SMA 20 semanas + EMA 21 semanas (calculadas en velas semanales).";
				Name						= "BullMarketSupportBand";
				Calculate					= Calculate.OnBarClose;
				IsOverlay					= true;
				DisplayInDataBox			= true;
				DrawOnPricePanel			= true;
				PaintPriceMarkers			= true;
				IsSuspendedWhileInactive	= true;

				SmaPeriod		= 20;
				EmaPeriod		= 21;
				FillBand		= true;
				BullBrush		= Brushes.LimeGreen;
				BearBrush		= Brushes.Red;
				FillOpacity		= 25;

				AddPlot(new Stroke(Brushes.LimeGreen, 2), PlotStyle.Line, "SMA20W");
				AddPlot(new Stroke(Brushes.DarkGreen, 2), PlotStyle.Line, "EMA21W");
			}
			else if (State == State.Configure)
			{
				// Serie semanal secundaria (BarsArray[1]).
				AddDataSeries(BarsPeriodType.Week, 1);
			}
			else if (State == State.DataLoaded)
			{
				weeklySma = SMA(BarsArray[1], SmaPeriod);
				weeklyEma = EMA(BarsArray[1], EmaPeriod);
			}
		}

		protected override void OnBarUpdate()
		{
			// Solo pintamos sobre la serie del gráfico.
			if (BarsInProgress != 0)
				return;

			if (CurrentBars[1] < Math.Max(SmaPeriod, EmaPeriod))
				return;

			Sma20W[0] = weeklySma[0];
			Ema21W[0] = weeklyEma[0];

			double upper = Math.Max(Sma20W[0], Ema21W[0]);
			double lower = Math.Min(Sma20W[0], Ema21W[0]);

			Brush regime = Close[0] >= lower ? BullBrush : BearBrush;
			if (Close[0] < upper && Close[0] >= lower)
				regime = Brushes.Gold; // precio dentro de la banda

			PlotBrushes[0][0] = regime;
			PlotBrushes[1][0] = regime;

			if (FillBand)
				Draw.Region(this, "BMSB" + CurrentBar, 1, 0, Sma20W, Ema21W, null, regime, FillOpacity);
		}

		#region Properties
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Sma20W => Values[0];

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Ema21W => Values[1];

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name = "SMA period (semanas)", Order = 1, GroupName = "Parameters")]
		public int SmaPeriod { get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name = "EMA period (semanas)", Order = 2, GroupName = "Parameters")]
		public int EmaPeriod { get; set; }

		[Display(Name = "Rellenar banda", Order = 3, GroupName = "Visual")]
		public bool FillBand { get; set; }

		[XmlIgnore]
		[Display(Name = "Color alcista", Order = 4, GroupName = "Visual")]
		public Brush BullBrush { get; set; }

		[Browsable(false)]
		public string BullBrushSerializable
		{
			get { return Serialize.BrushToString(BullBrush); }
			set { BullBrush = Serialize.StringToBrush(value); }
		}

		[XmlIgnore]
		[Display(Name = "Color bajista", Order = 5, GroupName = "Visual")]
		public Brush BearBrush { get; set; }

		[Browsable(false)]
		public string BearBrushSerializable
		{
			get { return Serialize.BrushToString(BearBrush); }
			set { BearBrush = Serialize.StringToBrush(value); }
		}

		[Range(0, 100)]
		[Display(Name = "Opacidad relleno (%)", Order = 6, GroupName = "Visual")]
		public int FillOpacity { get; set; }
		#endregion
	}
}
