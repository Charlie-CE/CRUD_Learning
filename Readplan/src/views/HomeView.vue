<template>
  <!-- <TheWelcome /> -->
  <main>
    <div>
      <button @click="CreaterUser">Create(建立)</button>
      <br />
    </div>
    <br />
    <hr />
    <br />
    <div>
      <button @click="GetSingleUser">Read(查詢)</button>
      <br />
    </div>
    <br />
    <hr />
    <br />
    <div>
      <button @click="GetAllUsers">Read(查詢全部)</button>
      <br />
    </div>
    <br />
    <hr />
    <br />
    <div>
      <button @click="UptUser">Update(更新)</button>
      <br />
    </div>
    <br />
    <hr />
    <br />
    <div>
      <button @click="DelUser">Delete(刪除)</button>
    </div>
    <br />
    <hr />
    <h3>請輸入id: <input type="number" min="1" max="99" v-model.number="UserId" /></h3>
    <br />
    <h3>請輸入Plan Name<input v-model="vPlan_name" /></h3>
    <h3>請輸入Planned Hours<input type="number" v-model.number="vPlanned_hours" /></h3>
    <h3>請輸入Actual Hours<input type="number" v-model.number="vActual_hours" /></h3>
    <h3>請輸入Abandoned<input type="checkbox" v-model="vAbandoned" /></h3>
    <br />

    <table class="data-table">
      <thead>
        <tr>
          <th>ID</th>
          <th>Plan Name</th>
          <th>Planned Hours</th>
          <th>Actual Hours</th>
          <th>Abandoned</th>
        </tr>
      </thead>
      <tbody>
        <!-- 2. 使用 v-for 渲染資料 -->
        <tr v-for="item in tableData" :key="item.id">
          <td>{{ item.id }}</td>
          <td>{{ item.plan_name }}</td>
          <td>${{ item.planned_hours }}</td>
          <td>${{ item.actual_hours }}</td>
          <td>${{ item.abandoned }}</td>
        </tr>
      </tbody>
    </table>
  </main>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {
  type User,
  type CreateUserRequest,
  getAllUser,
  getUser,
  deleteUser,
  createUser,
  updateUser,
} from '../api/userApi'

// 定義表格資料
const tableData = ref<User[]>([])
const loading = ref(false)
const UserId = ref<number | null>(null)
const vPlan_name = ref<string>()
const vPlanned_hours = ref<number>()
const vActual_hours = ref<number>()
const vAbandoned = ref<boolean>()

// 查詢所有用戶
const GetAllUsers = async () => {
  loading.value = true
  tableData.value = []

  try {
    const res = await getAllUser()
    tableData.value = res.data
    ref(tableData.value)
  } catch (err) {
    console.error('getUsers error:', err)
  } finally {
    loading.value = false
  }
}

// 查詢指定用戶
const GetSingleUser = async () => {
  loading.value = true
  tableData.value = []

  try {
    const res = await getUser(UserId.value)
    tableData.value.push(res.data)
    ref(tableData.value)
  } catch (err) {
    console.error('UserId.value:' + UserId.value)
    console.error('getUsers error:', err)
  } finally {
    loading.value = false
  }
}

// 查詢指定用戶
const DelUser = async () => {
  loading.value = true

  try {
    await deleteUser(UserId.value)
    alert('刪除資料成功')
  } catch (err) {
    console.error('UserId.value:' + UserId.value)
    console.error('getUsers error:', err)
  } finally {
    loading.value = false
  }
}

// 創建用戶
const CreaterUser = async () => {
  loading.value = true

  const user = ref<CreateUserRequest>({
    plan_name: vPlan_name.value,
    planned_hours: vPlanned_hours.value,
    actual_hours: vActual_hours.value,
    abandoned: vAbandoned.value,
  })

  try {
    await createUser(user.value)
    alert('創建用戶成功')
  } catch (err) {
    console.error('user.value:' + user.value)
    console.error('getUsers error:', err)
  } finally {
    loading.value = false
  }
}

// 更新用戶
const UptUser = async () => {
  loading.value = true

  const user = ref<CreateUserRequest>({
    plan_name: vPlan_name.value,
    planned_hours: vPlanned_hours.value,
    actual_hours: vActual_hours.value,
    abandoned: vAbandoned.value,
  })

  try {
    await updateUser(UserId.value, user.value)
    alert('更新用戶成功')
  } catch (err) {
    console.error('UserId.value:' + UserId.value)
    console.error('user.value:' + user.value)
    console.error('getUsers error:', err)
  } finally {
    loading.value = false
  }
}
</script>

<style>
.data-table {
  width: 100%;
  border-collapse: collapse;
}
.data-table th,
.data-table td {
  border: 2px solid #00ff;
  padding: 8px;
  text-align: left;
}
.data-table th {
  background-color: #f2f2;
}
</style>
