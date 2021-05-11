import { Modal, Button, Form } from 'react-bootstrap';

import { toast } from 'react-toastify';
import PropTypes from 'prop-types';

const NewTodoTypeModalComponent = ({ setTodoType, onHide, show }) => (
  <Modal
    onHide={onHide}
    size="md"
    aria-labelledby="contained-modal-title-vcenter"
    centered
    show={show}
  >
    <Modal.Header className="bg-egyedi color border-color" closeButton>
      <Modal.Title id="contained-modal-title-vcenter">
        <h5 className="text-center color">Új Tábla</h5>
      </Modal.Title>
    </Modal.Header>
    <Modal.Body className="bg-egyedi color">
      <Form onSubmit={
          (e) => {
            e.preventDefault();
            const item = {
              name: e.target.elements.name.value,
            };
            const data = new FormData();
            data.append('data', JSON.stringify(item));

            const requestOptions = {
              method: 'POST',
              body: data,
            };

            fetch(`${process.env.REACT_APP_API_URL}/api/todotype`, requestOptions)
              .then((response) => {
                if (!response.ok) {
                  toast.error('Sikertelen Hozzáadás!');
                  return undefined;
                }

                return response.json();
              })
              .then((todotype) => {
                if (todotype !== undefined) {
                  setTodoType((prev) => {
                    prev = { ...prev };
                    todotype.todoSize = 0;
                    prev[todotype.order] = todotype;
                    return prev;
                  });
                  toast.success('Sikeres hozzáadás!');
                }
              })
              .catch(() => {
                toast.error('Kommunikáció a szerverrel megszakadt!');
              });
            onHide();
          }
        }
      >
        <Form.Group controlId="name">
          <Form.Label>Táblanév:</Form.Label>
          <Form.Control type="text" name="name" placeholder="Írd be a tábla nevét" className="bg-egyedi color border-color" />
        </Form.Group>
        <Button variant="primary" type="submit">
          Küldés
        </Button>

      </Form>
    </Modal.Body>
    <Modal.Footer className="bg-egyedi border-color">
      <Button variant="secondary" onClick={onHide}>Bezár</Button>
    </Modal.Footer>
  </Modal>
);

NewTodoTypeModalComponent.propTypes = {
  setTodoType: PropTypes.func.isRequired,
  show: PropTypes.bool.isRequired,
  onHide: PropTypes.func.isRequired,
};

export default NewTodoTypeModalComponent;
