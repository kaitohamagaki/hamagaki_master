import numpy as np
import cv2
from PIL import Image
from keras.models import load_model

imsize = (64, 64)
some_threshold = 5000
keras_param = "F:\\MTG\\newcode\\walk_jump.h5"
model = load_model(keras_param)

def load_image_in_memory(image):
    img = image.convert('RGB')
    img = img.resize(imsize)
    img = np.asarray(img)
    img = img / 255.0
    return img

def save_frame_camera_cycle(cycle, delay=1, window_name='frame'):
    cap = cv2.VideoCapture(0)
    fgbg = cv2.createBackgroundSubtractorMOG2()

    if not cap.isOpened():
        return
    
    j = 0
    n = 0
    while True:
        ret, frame = cap.read()
        if not ret:
            break

        # 背景差分法を用いて前景を抽出
        fgmask = fgbg.apply(frame)

        # 前景の画素数を計算
        non_zero_count = cv2.countNonZero(fgmask)
        print(non_zero_count)

        if n == cycle:
            n = 0
            
            # リアルタイムの画像処理
            img = Image.fromarray(cv2.cvtColor(frame, cv2.COLOR_BGR2RGB))
            img = load_image_in_memory(img)
            prd = model.predict(np.array([img]))
            prelabel = np.argmax(prd, axis=1)
            if non_zero_count > some_threshold:
                if prelabel == 0:
                    print("walk")
                elif prelabel == 1:
                    print("jump")
            
            cv2.imshow(window_name, frame)
            if cv2.waitKey(delay) & 0xFF == ord('q'):
                break

        if j > 5000:
            break

        n += 1
        j += 1

    cap.release()
    cv2.destroyAllWindows()

save_frame_camera_cycle(cycle=1)
