import React, { useEffect, useState } from 'react';
import Plot from 'react-plotly.js';
import Button from '@mui/material/Button';

function StockTracker() {
    const [stockData, setStockData] = useState([]);
    const [ws, setWs] = useState(null);

    useEffect(() => {
        if (ws) {
            ws.onopen = () => {
                console.log('Connected to WebSocket');
            };

            ws.onmessage = (e) => {
                const updatedStockData = JSON.parse(e.data);
                setStockData(prevStockData => [...prevStockData, updatedStockData]);
            };

            return () => {
                ws.close();
            };
        }
    }, [ws]);

    const handleStart = () => {
        const newWs = new WebSocket('wss://localhost:7081/stocks');
        setWs(newWs);
    };

    const handleStop = () => {
        if (ws) {
            ws.close();
            setWs(null);
        }
    };

    // Extract prices and timestamps from stockData
    const prices = stockData.map(stock => stock.price);
    const timestamps = stockData.map(stock => stock.timestamp);

    const data = [
        {
            x: timestamps,
            y: prices,
            type: 'scatter',
            mode: 'lines+markers',
            marker: { color: 'red' },
            line: { color: 'grey' },
            hovertemplate: '<b>%{y}</b><br>%{x}',
        }
    ];

    return (
        <div className='mt-5'>
            <h1 className='display-5'>Real-Time Stock Tracker</h1>
            <div className='mt-3'>
                <Button variant="outlined" onClick={handleStart} style={{ marginRight: '20px' }}>Start Connection</Button>
                <Button variant="outlined" color="error" onClick={handleStop}>Stop Connection</Button>
            </div>
            <div className='mt-3'>
                <Plot
                    data={data}
                    layout={{
                        width: 1000,
                        height: 500,
                        // title: 'Stock Prices Over Time',
                        xaxis: { title: 'Timestamp' },
                        yaxis: { title: 'Price' }
                    }}
                />
            </div>
        </div>
    );
}

export default StockTracker;
