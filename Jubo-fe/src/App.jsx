import { useState, useEffect, useCallback } from 'react';
import {
  Typography, List, ListItem, ListItemText, Paper,
  Button, Dialog, DialogTitle, DialogContent, DialogActions,
  TextField, IconButton, CircularProgress, Alert, Box, Snackbar
} from '@mui/material';
import { Edit as EditIcon, Save as SaveIcon, Close as CloseIcon, AddComment as AddCommentIcon } from '@mui/icons-material';

const PATIENTS_API_URL = 'http://localhost:5043/Patients';
const ORDER_API_BASE = 'http://localhost:5043/Order';

const fetchPatients = async () => {
  console.log("API Call: 正在獲取病人列表...");
  try {
    const response = await fetch(PATIENTS_API_URL);
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    const data = await response.json();
    return { status: response.status, data: data };
  } catch (error) {
    console.error("獲取病人列表失敗:", error);
    return { status: 500, message: error.message };
  }
};

const fetchOrder = async (orderId) => {
  console.log(`API Call: 正在獲取單筆醫囑 ${orderId}...`);
  try {
    const response = await fetch(`${ORDER_API_BASE}/${orderId}`);
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    if (response.status === 204) {
      return { status: 204, data: null };
    }

    const text = await response.text();
    if (!text) {
      return { status: response.status, data: null };
    }

    const data = JSON.parse(text);

    // 為前端 UI 附加一個標記
    const orderWithEditable = {
      ...data,
      Editable: true
    };
    return { status: response.status, data: orderWithEditable };
  } catch (error) {
    console.error(`獲取醫囑 ${orderId} 失敗:`, error);
    return { status: 500, message: error.message };
  }
};

const saveOrder = async (patientUid, order) => {
  const payload = {
    id: order.id,
    uid: order.uid,
    message: order.message,
  };

  try {
    const response = await fetch(ORDER_API_BASE, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      let errorBody = await response.text();
      try { errorBody = JSON.parse(errorBody); } catch (e) { /* ignore parse error */ }
      throw new Error(`HTTP error! status: ${response.status}, message: ${errorBody.message || errorBody}`);
    }

    return {
      status: response.status,
      message: '醫囑更新成功',
      data: payload
    };

  } catch (error) {
    console.error("儲存醫囑失敗:", error);
    return { status: 500, message: error.message };
  }
};

const OrderManager = ({ patient, currentOrder, onOrdersUpdate, onClose }) => {
  const [message, setMessage] = useState('');
  const [isSaving, setIsSaving] = useState(false);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });

  useEffect(() => {
    if (currentOrder) {
      setMessage(currentOrder.message || '');
    } else {
      setMessage('');
    }
  }, [currentOrder]);

  const handleMessageChange = (e) => {
    setMessage(e.target.value);
  };

  const handleSave = async () => {
    // 檢查是否未修改
    if (currentOrder && message === currentOrder.message) {
      setSnackbar({ open: true, message: '醫囑內容未變更', severity: 'info' });
      return;
    }

    setIsSaving(true);

    const isNew = !currentOrder;
    const orderToSave = {
      id: currentOrder?.id,
      message: message,
      isNew: isNew,
    };

    try {
      const response = await saveOrder(patient.id, orderToSave);

      if (response.status >= 200 && response.status < 300) {
        setSnackbar({ open: true, message: response.message, severity: 'success' });

        const savedOrder = { ...response.data, Editable: true };

        onOrdersUpdate(patient.id, savedOrder, false);
        onClose();

      } else {
        setSnackbar({ open: true, message: `儲存失敗: ${response.message || '未知錯誤'}`, severity: 'error' });
      }
    } catch (error) {
      console.error("儲存醫囑時發生錯誤:", error);
      setSnackbar({ open: true, message: '儲存時發生例外錯誤', severity: 'error' });
    } finally {
      setIsSaving(false);
    }
  };


  return (
    <Dialog open={true} onClose={onClose} fullWidth maxWidth="md">
      <DialogTitle className="flex justify-between items-center text-xl font-semibold">
        編輯 {patient.name} 的醫囑
      </DialogTitle>

      <DialogContent dividers className="min-h-[30vh] p-4 bg-gray-50">

        {!currentOrder && (
          <Alert severity="info" className="mb-4">
            目前沒有醫囑。請在下方輸入框新增。
          </Alert>
        )}

        {currentOrder && (
          <Typography variant="caption" color="textSecondary" className="font-mono">
            醫囑 ID: {currentOrder.uid}
          </Typography>
        )}

        <TextField
          fullWidth
          multiline
          rows={8}
          value={message}
          onChange={handleMessageChange}
          variant="outlined"
          size="small"
          margin="dense"
          label="醫囑內容"
          autoFocus
          className="mt-2"
        />

      </DialogContent>
      <DialogActions className="p-3 flex justify-between">
        <Box>
          <Button onClick={onClose} color="inherit" variant="outlined" startIcon={<CloseIcon />} className="mr-2">
            取消
          </Button>
          <Button
            onClick={handleSave}
            color="primary"
            variant="contained"
            startIcon={isSaving ? <CircularProgress size={20} color="inherit" /> : <SaveIcon />}
            disabled={isSaving || (currentOrder && message === currentOrder.message)}
          >
            {isSaving ? "儲存中..." : (currentOrder ? "儲存變更" : "新增醫囑")}
          </Button>
        </Box>
      </DialogActions>
      <Snackbar
        open={snackbar.open}
        autoHideDuration={4000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
        anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
      >
        <Alert onClose={() => setSnackbar({ ...snackbar, open: false })} severity={snackbar.severity} sx={{ width: '100%' }}>
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Dialog>
  );
};

const App = () => {
  const [patients, setPatients] = useState([]);
  const [ordersMap, setOrdersMap] = useState({});
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [isLoadingPatients, setIsLoadingPatients] = useState(true);
  const [isLoadingOrders, setIsLoadingOrders] = useState(false);
  const [error, setError] = useState(null);


  useEffect(() => {
    const loadPatients = async () => {
      try {
        const response = await fetchPatients();
        if (response.status === 200) {
          const sanitizedPatients = response.data.map(p => ({
            id: p.id,
            uid: p.uid,
            name: p.name,
            orderId: p.orderId || null,
          }));
          setPatients(sanitizedPatients);
        } else {
          setError(`無法載入病人資料: ${response.message || response.status}`);
        }
      } catch (err) {
        setError('API 連線失敗，請檢查後端服務是否運行於 http://localhost:5043');
      } finally {
        setIsLoadingPatients(false);
      }
    };
    loadPatients();
  }, []);

  const handlePatientClick = useCallback(async (patient) => {
    setSelectedPatient(patient);
    setIsLoadingOrders(true);
    setError(null);

    if (ordersMap[patient.id] !== undefined) {
      setIsLoadingOrders(false);
      return;
    }

    if (!patient.orderId) {
      setOrdersMap(prev => ({
        ...prev,
        [patient.id]: null,
      }));
      setIsLoadingOrders(false);
      return;
    }

    try {
      const response = await fetchOrder(patient.orderId);
      if (response.status === 200) {
        setOrdersMap(prev => ({
          ...prev,
          [patient.id]: response.data,
        }));
      } else {
        setError(`無法載入 ${patient.name} 的醫囑: ${response.message || response.status}`);
        setOrdersMap(prev => ({ ...prev, [patient.id]: null }));
      }
    } catch (err) {
      setError('載入醫囑時網路錯誤');
    } finally {
      setIsLoadingOrders(false);
    }
  }, [ordersMap]);

  const handleOrdersUpdate = useCallback((patientId, updatedOrder, isDeleted = false) => {

    setOrdersMap(prev => ({
      ...prev,
      [patientId]: updatedOrder,
    }));

    setPatients(prevPatients => prevPatients.map(p => {
      if (p.id === patientId) {
        if (isDeleted) {
          return { ...p, orderId: null };
        } else if (updatedOrder) {
          return { ...p, orderId: updatedOrder.id };
        }
      }
      return p;
    }));
  }, []);

  const handleCloseDialog = useCallback(() => {
    setSelectedPatient(null);
  }, []);

  return (
    <div className="min-h-screen bg-gray-100 p-4 md:p-8 font-sans">
      <div className="max-w-4xl mx-auto">
        <Typography
          variant="h4"
          component="h1"
          className="text-center mb-6 font-bold text-blue-600"
        >
          住民醫囑管理系統 (Patient Order Management)
        </Typography>

        <Paper elevation={4} className="p-4 md:p-6 rounded-xl shadow-2xl">
          <Typography
            variant="h5"
            className="mb-4 pb-2 border-b-2 border-blue-500 font-semibold text-gray-800"
          >
            住民列表
          </Typography>

          {error && <Alert severity="error" className="mb-4">{error}</Alert>}

          {isLoadingPatients ? (
            <Box className="flex justify-center items-center py-10">
              <CircularProgress />
              <Typography className="ml-3 text-blue-500">正在載入病人資料...</Typography>
            </Box>
          ) : (
            <List className="p-0">
              {patients.length === 0 ? (
                <Alert severity="warning">
                  無法載入病人資料。請確保後端服務正在運行於 http://localhost:5043/Patients 並返回正確格式。
                </Alert>
              ) : (
                patients.map((patient) => (
                  <ListItem
                    key={patient.id}
                    button
                    onClick={() => handlePatientClick(patient)}
                    className="mb-2 rounded-lg transition duration-300 ease-in-out hover:bg-blue-50 shadow-sm border border-gray-200"
                  >
                    <ListItemText
                      primary={patient.name}
                      secondary={`ID: ${patient.uid} / OrderID: ${patient.orderId || 'N/A'}`} // 顯示 orderId
                      primaryTypographyProps={{ variant: 'h6', className: 'text-gray-800' }}
                      secondaryTypographyProps={{ className: 'text-sm text-gray-500 font-mono' }}
                    />
                    <Button
                      variant="contained"
                      color={patient.orderId ? "primary" : "secondary"}
                      size="small"
                      startIcon={patient.orderId ? <EditIcon /> : <AddCommentIcon />}
                      className="hidden sm:inline-flex"
                    >
                      {patient.orderId ? "查看/編輯醫囑" : "新增醫囑"}
                    </Button>
                  </ListItem>
                ))
              )}
            </List>
          )}
        </Paper>
      </div>

      {selectedPatient && (
        <OrderManager
          patient={selectedPatient}
          currentOrder={ordersMap[selectedPatient.id]}
          onOrdersUpdate={handleOrdersUpdate}
          onClose={handleCloseDialog}
        />
      )}

      <Dialog open={isLoadingOrders} maxWidth="xs" fullWidth>
        <DialogContent className="flex justify-center items-center p-8">
          <CircularProgress className="mr-3" />
          <Typography>正在載入醫囑...</Typography>
        </DialogContent>
      </Dialog>

    </div>
  );
};

export default App;