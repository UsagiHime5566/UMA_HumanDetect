import tensorflow as tf

msg = tf.constant('Hello, TensorFlow!')
tf.print(msg)


"""

import tensorflow.compat.v1 as tf

tf.disable_v2_behavior()

# constant可以視為tf專用的變數型態
# 其他包括Variable，placeholder
A = tf.constant('Hello World!')

# 使用 with 可以讓Session自動關閉
with tf.Session() as sess:

    # 在 tensorflow內要使用run，才會讓計算圖開始執行
    B = sess.run(A)

    print(B)

"""