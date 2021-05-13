import React from "react";
import { ModalContext } from "./App";

import InviteLecturerModal from "modals/InviteLecturer";
import CourseWorkCreateModal from "modals/CourseWorkCreate";

export default function ModalRoot() {
  return (
    <ModalContext.Consumer>
      {(value) => {
        const modalProps = {
          open: true,
          onClose: value.closeModal,
          ...value.state.props,
        };
        switch (value.state.type) {
          case "INVITE_LECTURER":
            return <InviteLecturerModal {...modalProps} />;
          case "COURSE_WORK_CREATE":
            return <CourseWorkCreateModal {...modalProps} />;
        }
      }}
    </ModalContext.Consumer>
  );
}
