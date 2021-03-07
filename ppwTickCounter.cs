#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
	public class ppwTickCounter : Indicator
	{

		private SimpleFont font = new SimpleFont("ARIAL", 13);

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Tick counter next to forming bar";
				Name										= "ppwTickCounter";
				Calculate									= Calculate.OnEachTick;
				IsOverlay									= false;
				DisplayInDataBox							= false;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= false;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{
			UpdateTickCounter();
		}

		private void UpdateTickCounter()
        {
			if (BarsPeriod == null || BarsPeriodType.Tick != BarsPeriod.BarsPeriodType)
            {
				return;
            }
			int tickCount = BarsPeriod.Value - Bars.TickCount;
			Draw.Text(this, "tickCount", false, "" + tickCount, -3, Close[0], 0, Brushes.White, font, TextAlignment.Center, Brushes.Transparent, Brushes.Transparent, 0);
        }
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private ppwTickCounter[] cacheppwTickCounter;
		public ppwTickCounter ppwTickCounter()
		{
			return ppwTickCounter(Input);
		}

		public ppwTickCounter ppwTickCounter(ISeries<double> input)
		{
			if (cacheppwTickCounter != null)
				for (int idx = 0; idx < cacheppwTickCounter.Length; idx++)
					if (cacheppwTickCounter[idx] != null &&  cacheppwTickCounter[idx].EqualsInput(input))
						return cacheppwTickCounter[idx];
			return CacheIndicator<ppwTickCounter>(new ppwTickCounter(), input, ref cacheppwTickCounter);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.ppwTickCounter ppwTickCounter()
		{
			return indicator.ppwTickCounter(Input);
		}

		public Indicators.ppwTickCounter ppwTickCounter(ISeries<double> input )
		{
			return indicator.ppwTickCounter(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.ppwTickCounter ppwTickCounter()
		{
			return indicator.ppwTickCounter(Input);
		}

		public Indicators.ppwTickCounter ppwTickCounter(ISeries<double> input )
		{
			return indicator.ppwTickCounter(input);
		}
	}
}

#endregion
