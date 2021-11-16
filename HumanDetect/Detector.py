import socket
import base64
import numpy as np
import cv2
import threading
import imutils
import time

# turn on this to release
isRelease = False

"""
先裝anaconda 絕對不會出錯, cmd執行時先cd到anaconda/envs 裡的python環境, 再打上 python file.py
example:
cd C:/Users/moca2018_b/anaconda3/envs/py38
python C:/Users/moca2018_b/Documents/GitHub/ArtTank/HumanDetect/human-counting-project-code.py


需求:
pip install opencv-python
pip install imutils
pip install numpy
"""

# detector config

# 多少次有人後, 送出有人訊號
active_times = 3

# 有效框位置，右正x,下正y
box_x = 345
box_y = 320
box_w = 135
box_h = 200



# init detector
active_rect = [box_x - box_w, box_y - box_h, box_w * 2, box_h * 2]
active_stack = 0
current_frame = None
HOGCV = cv2.HOGDescriptor()
HOGCV.setSVMDetector(cv2.HOGDescriptor_getDefaultPeopleDetector())


# socket config
HOST = '127.0.0.1'
PORT = 25568
clientMessage = '5566[/TCP]'
buffersize = 32768
# char count of [/TCP]
tcpendCount = 6


print("Prepare to start ...")
time.sleep(10)
print("Start Client proccess")

# start socket
client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client.connect((HOST, PORT))


def pointInRect(point, rect):
    x1, y1, w, h = rect
    x2, y2 = x1+w, y1+h
    x, y = point
    if (x1 < x and x < x2):
        if (y1 < y and y < y2):
            return True
    return False

def cv_show_window():
    cv2.imshow('output', current_frame)
    cv2.waitKey(0)

def detect(frame):

    # winStride , scale 參數越大，偵測速度越快 但越不準
    # 參考 https://www.pyimagesearch.com/2015/11/16/hog-detectmultiscale-parameters-explained/
    bounding_box_cordinates, weights =  HOGCV.detectMultiScale(frame, winStride = (8, 8), padding = (8, 8), scale = 1.05)

    if not isRelease:
        # 有效框顯示
        cv2.rectangle(frame, (box_x - box_w, box_y - box_h), (box_x + box_w, box_y + box_h), (0,255,255), 2)
    
    person = 0
    active_person = 0
    for x,y,w,h in bounding_box_cordinates:
        p_pos = (x + w//2, y + h//2)
        if pointInRect(p_pos, active_rect):
            active_person += 1

        print("detect person in " + str(p_pos[0]) + "," + str(p_pos[1]))

        if not isRelease:
            cv2.rectangle(frame, (x,y), (x+w,y+h), (0,255,0), 2)
            cv2.putText(frame, f'person {person} at {p_pos[0]},{p_pos[1]}', (x,y), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0,0,255), 1)
            cv2.circle(frame, p_pos, 2, (0, 0, 255), 2)
            person += 1

    print("active person num :" + str(active_person))
    global active_stack
    if active_person > 0:
        active_stack = min(active_times, active_stack + 1)
        if active_stack >= active_times:
            client.send(clientMessage.encode())
    else:
        active_stack = max(0, active_stack - 1)

    if not isRelease:
        cv2.putText(frame, 'Status : Detecting ', (40,40), cv2.FONT_HERSHEY_DUPLEX, 0.8, (255,0,0), 2)
        cv2.putText(frame, f'Total Persons : {person}', (40,70), cv2.FONT_HERSHEY_DUPLEX, 0.8, (255,0,0), 2)
        global current_frame
        current_frame = frame
        # cv2.imshow('output', frame)

    return frame


def detectByPathImage(image):
    image = imutils.resize(image, width = min(800, image.shape[1])) 
    result_image = detect(image)

    print("detect finished")

    # if not isRelease:
    #     cv2.waitKey(0)
    #     cv2.destroyAllWindows()


def handle_send():
    while True:
        content = input()
        client.send(content.encode())

def handle_receive():
    while True:
        print("wait for message...")
        buf = client.recv(buffersize).decode('utf-8')
        i = 0
        while buf[-tcpendCount:] != "[/TCP]":
            buf += client.recv(buffersize).decode('utf-8')
            #print("continue recv " + str(i) + " : " + buf[-tcpendCount:])
            i = i+1
        
        stringdata = buf[:-tcpendCount]
        img_b64decode = base64.b64decode(stringdata)
        img_array = np.fromstring(img_b64decode,np.uint8) # 轉換np序列
        img=cv2.imdecode(img_array,cv2.COLOR_BGR2RGB)  # 轉換Opencv格式

        detectByPathImage(img)
        #cv2.imshow("img",img)
        #cv2.waitKey()

send_handler = threading.Thread(target=handle_send,args=())
send_handler.start()
receive_handler = threading.Thread(target=handle_receive,args=())
receive_handler.start()

if not isRelease:
    while True:
        if current_frame is None:
            time.sleep(0.1)
            continue

        cv2.imshow('output', current_frame)
        cv2.waitKey(10)