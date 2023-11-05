import asyncio
import websockets
import json
import time

async def send_websocket_message():
    # WebSocket服务器地址，替换为您的本地WebSocket服务器地址
    uri = "ws://localhost:8080"

    # 要发送的消息
    message_start = {
        "messageType": "COMPETITION_CONTROL_COMMAND",
        "token": "string",
        "command": "START"
    }
    message_end = {
        "messageType": "COMPETITION_CONTROL_COMMAND",
        "token": "string",
        "command": "END"
    }
    message_reset = {
        "messageType": "COMPETITION_CONTROL_COMMAND",
        "token": "string",
        "command": "RESET"
    }
    async with websockets.connect(uri) as websocket:
            # 将消息转换为JSON格式并发送
            await websocket.send(json.dumps(message_start))
            print(f"Sent message: {json.dumps(message_start)}")

            # 等待服务器的响应（可选）
            response = await websocket.recv()
            print(f"Received response: {response}")

            time.sleep(1)

            await websocket.send(json.dumps(message_end))
            print(f"Sent message: {json.dumps(message_end)}")

            response = await websocket.recv()
            print(f"Received response: {response}")

            time.sleep(1)

            await websocket.send(json.dumps(message_reset))
            print(f"Sent message: {json.dumps(message_reset)}")

            response = await websocket.recv()
            print(f"Received response: {response}")

            time.sleep(1)

            await websocket.send(json.dumps(message_start))
            print(f"Sent message: {json.dumps(message_start)}")

            response = await websocket.recv()
            print(f"Received response: {response}")

            time.sleep(1)

def main():
    asyncio.get_event_loop().run_until_complete(send_websocket_message())
    print("Over")

if __name__ == "__main__":
    main()
