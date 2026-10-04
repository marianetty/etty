# Bull Market Support Band (BMSB) para NinjaTrader 8

NinjaTrader no trae la BMSB de serie. Este indicador la añade.

- **SMA de 20 semanas** + **EMA de 21 semanas**, calculadas siempre sobre velas semanales
  (funciona aunque el gráfico esté en 1h, 4h, diario, etc.).
- Color: verde = precio por encima de la banda, dorado = dentro, rojo = por debajo.

## Instalación

1. Copia `BullMarketSupportBand.cs` en
   `Documents\NinjaTrader 8\bin\Custom\Indicators\`.
2. En NinjaTrader: *New → NinjaScript Editor*, abre el archivo y pulsa **F5** para compilar.
3. En el gráfico: *Indicators… → BullMarketSupportBand*.

## Notas para cripto

- Datos: conexión Coinbase (spot) o futuros CME (BTC/MBT, ETH/MET).
- Usa la plantilla de horario **"Default 24 x 7"** para que las velas semanales salgan bien.
- Carga suficiente histórico (*Days to load* ≥ 365) para que la EMA de 21 semanas se estabilice.
- Si el gráfico va lento en timeframes muy pequeños, desactiva *Rellenar banda*.
