//＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
//未ログイン対策
//＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

if (!localStorage.getItem("userName")) {

    alert("ログインしてください");
    window.location.href = "Login.html";

} else { 

    //ストレージのUserNameを読出
    const loginUserName = localStorage.getItem("userName") ?? "";
    console.info(`ログイン成功 UserName=${loginUserName}`);

    //＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    //チャット表示更新
    //＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    //チャット吹き出し作成
    function addMessage(chat) {

        //チャットエリア取得
        const chatArea = document.getElementById("chatMessages");

        //メッセージ1件分のdiv作成
        const row = document.createElement("div");

        //自分か相手か判定
        if (chat.userName === loginUserName) {
            row.className = "message-row mine";
        } else {
            row.className = "message-row other";
        }

        //チャット用HTML組み立て
        const dateTime = new Date(chat.createdAt)
            .toLocaleString("ja-JP", {
                year: "numeric",
                month: "2-digit",
                day: "2-digit",
                hour: "2-digit",
                minute: "2-digit"
            });

        row.innerHTML = `
            <div class="message-bubble">
                <div class="message-name">${chat.userName}</div>
                <div class="message-text">${chat.message}</div>
                <div class="message-time">${dateTime}</div>
            </div>
        `;

        //チャット欄に追加
        chatArea.appendChild(row);

    }

    //チャット更新
    async function loadChats() {

        //チャットエリア取得
        const chatArea = document.getElementById("chatMessages");

        try {

            //GET送信
            const response = await fetch("/api/chats");

            //取得に成功しなかったら終了
            if (response.status !== 200) {
                console.warn("チャット取得失敗");
                return;
            }

            //レスポンスのチャット全件データをJSON変換
            const chats = await response.json();

            const chatArea = document.getElementById("chatMessages");
            //既存メッセージ削除
            chatArea.innerHTML = "";
            //チャット全件描画
            chats.forEach(chat => {addMessage(chat);});

        }
        catch (error) {
            console.error("チャット取得通信失敗", error);
        }
    }

    // 初回表示
    loadChats();

    // 1秒ごとに更新
    setInterval(loadChats, 5000);


    //＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    //チャット送信ボタン押下処理
    //＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    document.getElementById("sendButton").addEventListener("click", async () => {

        console.info("送信ボタン押下");

        //入力取得
        const message = document.getElementById("messageInput").value;

        //チャットエリア取得
        const chatArea = document.getElementById("chatMessages");

        //メッセージが空白・Nullであれば終了
        if (message.trim() === "") {
            alert("メッセージを入力してください");
            return;
        }

        try {
            //送信データ整形
            const sendMessageData = {
                userName: loginUserName,
                message: message
            };

            //POST送信
            const response = await fetch("/api/chats", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(sendMessageData)
            });

            if (response.status === 201) {
                console.info("チャット送信成功");

                //テキストボックス空白にして調整
                document.getElementById("messageInput").value = "";
                document.getElementById("messageInput").style.height = "44px";

                //チャットリロード
                loadChats();

                // 最新メッセージへスクロール
                const chatArea = document.getElementById("chatMessages");
                chatArea.scrollTop = chatArea.scrollHeight;

            }
            else if (response.status === 400) {
                alert("入力値が不正で送信できませんでした");
            }
            else if (response.status === 500) {
                alert("サーバーエラーで送信できませんでした");
            }

        }
        catch (error) {
            console.error("チャット送信通信失敗", error);
            alert("通信に失敗しました");
        }

    });

    //＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝
    //ログアウト
    //＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝＝

    document.getElementById("logoutButton").addEventListener("click", () => {

        //データ削除してログイン画面に戻る
        localStorage.removeItem("userId");
        localStorage.removeItem("userName");
        window.location.href = "Login.html";

    }); 
  

}