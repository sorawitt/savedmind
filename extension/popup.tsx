import { useState } from "preact/hooks"

function IndexPopup() {
    const [url, setUrl] = useState("")

    const handleSave = async () => {
        const [tab] = await chrome.tabs.query({ active: true, currentWindow: true })
        if (tab?.url) {
            setUrl(tab.url)
            // TODO: Send to backend API
            console.log("Saving:", tab.url, tab.title)
        }
    }

    return (
        <div style={{ padding: "16px", minWidth: "300px", fontFamily: "system-ui" }}>
            <h2 style={{ margin: "0 0 12px 0", fontSize: "18px" }}>SavedMind</h2>
            <button
                onClick={handleSave}
                style={{
                    width: "100%",
                    padding: "10px",
                    backgroundColor: "#6366f1",
                    color: "white",
                    border: "none",
                    borderRadius: "6px",
                    cursor: "pointer",
                    fontSize: "14px"
                }}
            >
                Save This Page
            </button>
            {url && (
                <p style={{ marginTop: "12px", fontSize: "12px", color: "#666" }}>
                    Saved: {url}
                </p>
            )}
        </div>
    )
}

export default IndexPopup
