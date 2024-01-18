import "./App.css";
import Axios from "axios";
import { useEffect, useState } from "react";

function App() {
    const [urlToShorten, setUrlToShorten] = useState("");
    const [shortenedUrl, setShortenedUrl] = useState("");

    const fetchShortUrl = () => {
        var requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ url: urlToShorten })
        };

        fetch('http://localhost:5000/shorten', requestOptions)
            .then(r=>r.json())
            .then(res=> setShortenedUrl(res))
    };

    return (
        <div className="App">
            <h1>URL Shortening Service</h1>
            <div className="input-wrapper">
                <input 
                    placeholder="insert url to shorten..."
                    onChange={(event) => {
                        setUrlToShorten(event.target.value);
                    }}
                />
            </div>
            <button onClick={() => fetchShortUrl()}> shorten url</button>

            <p> {shortenedUrl} </p>
        </div>
    );
}

export default App;