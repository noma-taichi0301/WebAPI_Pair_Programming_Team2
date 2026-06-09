function addMessage(chat) {

    const chatArea = document.getElementById("chatMessages");
    const row = document.createElement("div");

    if (chat.userId === loginUserId) {
        row.className = "message-row mine";
    } else {
        row.className = "message-row other";
    }

    row.innerHTML = `
        <div class="message-bubble">
            <div class="message-name">${chat.userName}</div>
            <div class="message-text">${chat.message}</div>
            <div class="message-time">${chat.createdAt}</div>
        </div>
    `;

    chatArea.appendChild(row);

    // 最新メッセージへスクロール
    chatArea.scrollTop = chatArea.scrollHeight;
}


//テキストボックス調整
const textarea = document.getElementById("messageInput");

textarea.addEventListener("input", () => {

    textarea.style.height = "44px";

    textarea.style.height = textarea.scrollHeight + "px";

});