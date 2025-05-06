import React, { useState } from "react";

function GetInputComponent() {
    const [inputValue, setInputValue] = useState("");

    const handleClick = () => {
        alert("Input value: " + inputValue);
    };

    <div>
        <input
            type="text"
            value={inputValue}
            onChange={(e) => setInputValue(e.target.value)}
        />
        <button onClick={handleClick}>Click</button>
    </div>

    return (
        inputValue
    );
}

export default GetInputComponent;