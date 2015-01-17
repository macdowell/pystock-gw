using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PyStock.SmartCOM
{
    using SmartCOM3Lib;

    class Connector
    {
        private StServerClass st_client = null;

        public Connector()
        {
            this.st_client = new StServerClass();

            this.st_client.AddBar += new _IStClient_AddBarEventHandler(EeventHandler_OnAddBar);
            this.st_client.AddTick += new _IStClient_AddTickEventHandler(EeventHandler_OnAddTick);
            this.st_client.AddTrade += new _IStClient_AddTradeEventHandler(EeventHandler_OnAddTrade);
            this.st_client.AddSymbol += new _IStClient_AddSymbolEventHandler(EeventHandler_OnAddSymbol);
            this.st_client.AddPortfolio += new _IStClient_AddPortfolioEventHandler(EeventHandler_OnAddPortfolio);
            this.st_client.AddTickHistory += new _IStClient_AddTickHistoryEventHandler(EeventHandler_OnAddTickHistory);

            this.st_client.SetMyOrder += new _IStClient_SetMyOrderEventHandler(EeventHandler_OnSetMyOrder);
            this.st_client.SetMyTrade += new _IStClient_SetMyTradeEventHandler(EeventHandler_OnSetMyTrade);
            this.st_client.SetPortfolio += new _IStClient_SetPortfolioEventHandler(EeventHandler_OnSetPortfolio);
            this.st_client.SetMyClosePos += new _IStClient_SetMyClosePosEventHandler(EeventHandler_OnSetMyClosePos);

            this.st_client.UpdateOrder += new _IStClient_UpdateOrderEventHandler(EeventHandler_OnUpdateOrder);
            this.st_client.UpdateQuote += new _IStClient_UpdateQuoteEventHandler(EeventHandler_OnUpdateQuote);
            this.st_client.UpdateBidAsk += new _IStClient_UpdateBidAskEventHandler(EeventHandler_OnUpdateBidAsk);
            this.st_client.UpdatePosition += new _IStClient_UpdatePositionEventHandler(EeventHandler_OnUpdatePosition);

            // The events of the commands
            this.st_client.Connected += new _IStClient_ConnectedEventHandler(this.EeventHandler_OnConnected);
            this.st_client.Disconnected += new _IStClient_DisconnectedEventHandler(this.EeventHandler_OnDisconnected);

            this.st_client.OrderFailed += new _IStClient_OrderFailedEventHandler(EeventHandler_OnOrderFailed);
            this.st_client.OrderSucceeded += new _IStClient_OrderSucceededEventHandler(EeventHandler_OnOrderSucceeded);
            this.st_client.OrderMoveFailed += new _IStClient_OrderMoveFailedEventHandler(EeventHandler_OnOrderMoveFailed);
            this.st_client.OrderMoveSucceeded += new _IStClient_OrderMoveSucceededEventHandler(EeventHandler_OnOrderMoveSucceeded);
            this.st_client.OrderCancelFailed += new _IStClient_OrderCancelFailedEventHandler(EeventHandler_OnOrderCancelFailed);
            this.st_client.OrderCancelSucceeded += new _IStClient_OrderCancelSucceededEventHandler(EeventHandler_OnOrderCancelSucceeded);            
        }

        protected StServerClass client 
        {
            get { return this.st_client; }
        }

        #region Processing Command
        protected void Connect(string address, UInt16 port, string login, string password)
        {
            this.client.connect(address, port, login, password);
        }

        protected void Disconect()
        {
            this.client.disconnect();
        }
        #endregion


        #region SmartCOM event handlers
        protected void EeventHandler_OnConnected()
        {
        }

        protected void EeventHandler_OnDisconnected(string reason)
        {
        }

        protected void EeventHandler_OnOrderFailed(int cookie, string orderid, string reason)
        {
        }

        protected void EeventHandler_OnOrderSucceeded(int cookie, string orderid)
        {
        }

        protected void EeventHandler_OnOrderMoveFailed(string orderid)
        {
        }
        protected void EeventHandler_OnOrderMoveSucceeded(string orderid)
        {
        }

        protected void EeventHandler_OnOrderCancelFailed(string orderid)
        {
        }
        protected void EeventHandler_OnOrderCancelSucceeded(string orderid)
        {
        }

        protected void EeventHandler_OnAddBar(int row, int nrows, string symbol, StBarInterval interval, DateTime datetime, double open, double high, double low, double close, double volume, double open_int)
        {
        }

        protected void EeventHandler_OnAddTick(string symbol, DateTime datetime, double price, double volume, string tradeno, StOrder_Action action)
        {
        }

        protected void EeventHandler_OnAddTrade(string portfolio, string symbol, string orderid, double price, double amount, System.DateTime datetime, string tradeno)
        {
        }

        protected void EeventHandler_OnAddSymbol(int row, int nrows, string symbol, string short_name, string long_name, string type, int decimals, int lot_size, double punkt, double step, string sec_ext_id, string sec_exch_name, System.DateTime expiry_date, double days_before_expiry, double strike)
        {
        }

        protected void EeventHandler_OnAddTickHistory(int row, int nrows, string symbol, DateTime datetime, double price, double volume, string tradeno, StOrder_Action action)
        {
        }

        protected void EeventHandler_OnAddPortfolio(int row, int nrows, string portfolioName, string portfolioExch, SmartCOM3Lib.StPortfolioStatus portfolioStatus)
        {
        }

        protected void EeventHandler_OnSetMyOrder(int row, int nrows, string portfolio, string symbol, StOrder_State state, StOrder_Action action, StOrder_Type type, StOrder_Validity validity, double price, double amount, double stop, double filled, DateTime datetime, string id, string no, int cookie)
        {
        }

        protected void EeventHandler_OnSetMyTrade(int row, int nrows, string portfolio, string symbol, DateTime datetime, double price, double volume, string tradeno, StOrder_Action buysell, string orderno)
        {
        }

        protected void EeventHandler_OnSetPortfolio(string portfolio, double cash, double leverage, double comission, double saldo)
        {
        }

        protected void EeventHandler_OnSetMyClosePos(int row, int nrows, string portfolio, string symbol, double amount, double price_buy, double price_sell, DateTime postime, string buy_order, string sell_order)
        {
        }

        protected void EeventHandler_OnUpdateOrder(string portfolio, string symbol, StOrder_State state, StOrder_Action action, StOrder_Type type, StOrder_Validity validity, double price, double amount, double stop, double filled, DateTime datetime, string orderid, string orderno, int status_mask, int cookie)
        {
        }

        protected void EeventHandler_OnUpdateQuote(string symbol, DateTime datetime, double open, double high, double low, double close, double last, double volume, double size, double bid, double ask, double bidsize, double asksize, double open_int, double go_buy, double go_sell, double go_base, double go_base_backed, double high_limit, double low_limit, int trading_status, double volat, double theor_price)
        {
        }

        protected void EeventHandler_OnUpdateBidAsk(string symbol, int row, int nrows, double bid, double bidsize, double ask, double asksize)
        {
        }

        protected void EeventHandler_OnUpdatePosition(string portfolio, string symbol, double avprice, double amount, double planned)
        {
        }
        #endregion
    }
}
